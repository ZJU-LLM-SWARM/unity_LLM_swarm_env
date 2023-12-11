import pandas as pd
import openai
import os
import json

openai_api_key = os.environ["OPENAI_API_KEY"]

# 创建一个稍微复杂的DataFrame，包含多种数据类型
df_complex = pd.DataFrame({
    'Name': ['Alice', 'Bob', 'Charlie'],
    'Age': [25, 30, 35],
    'Salary': [50000.0, 100000.5, 150000.75],
    'IsMarried': [True, False, True]
})

df_complex_json = df_complex.to_json(orient='split')

def calculate_total_age_from_split_json(input_json):
    """
    从给定的JSON格式字符串（按'split'方向排列）中解析出DataFrame，计算所有人的年龄总和，并以JSON格式返回结果。

    参数:
    input_json (str): 包含个体数据的JSON格式字符串。

    返回:
    str: 所有人的年龄总和，以JSON格式返回。
    """

    # 将JSON字符串转换为DataFrame
    df = pd.read_json(input_json, orient='split')

    # 计算所有人的年龄总和
    total_age = df['Age'].sum()

    # 将结果转换为字符串形式，然后使用json.dumps()转换为JSON格式
    return json.dumps({"total_age": str(total_age)})

# 使用函数计算年龄总和，并以JSON格式输出
result = calculate_total_age_from_split_json(df_complex_json)
print("The JSON output is:", result)

function_repository = {
            "calculate_total_age_from_split_json": calculate_total_age_from_split_json,
        }

calculate_total_age_from_split_json = {"name": "calculate_total_age_from_split_json",
                                       "description": "计算年龄总和的函数，从给定的JSON格式字符串（按'split'方向排列）中解析出DataFrame，计算所有人的年龄总和，并以JSON格式返回结果。",
                                       "parameters": {"type": "object",
                                       "properties": {"input_json": {"type": "string",
                                                                     "description": "执行计算年龄总和的数据集"},
                                                   },
                                        "required": ["input_json"],
                                    },
                     }

functions = [calculate_total_age_from_split_json]

messages=[
    {"role": "system", "content": "你是一位优秀的数据分析师, 现在有这样一个数据集input_json：%s，数据集以JSON形式呈现" % df_complex_json},
    {"role": "user", "content": "请在数据集input_json上执行计算所有人年龄总和函数"}
]


response = openai.ChatCompletion.create(
  model="gpt-3.5-turbo-16k-0613",
  messages=messages,
  functions = functions,
  function_call = "auto",
)

#########################################
# 这一步不返回function_call，需要手动解析，还需要改


print(response["choices"][0]["message"].content)

# 保存交互过程中的函数名称
function_name = response["choices"][0]["message"]["function_call"]["name"]

# 加载交互过程中的参数
function_args = json.loads(response["choices"][0]["message"]["function_call"]["arguments"])

# 保存具体的函数对象
local_fuction_call = function_repository[function_name]

# 调用函数
final_response = local_fuction_call(**function_args)

# 追加第一次模型返回结果消息
messages.append(response["choices"][0]["message"])

# 追加function计算结果，注意：function message必须要输入关键词name
messages.append({"role": "function", "name": function_name, "content": final_response,})

last_response = openai.ChatCompletion.create(
            model="gpt-3.5-turbo-16k-0613",
            messages=messages,)
print(last_response["choices"][0]["message"]["content"])



