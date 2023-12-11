import os
from langchain.chat_models import ChatOpenAI
from langchain.schema import HumanMessage, SystemMessage, AIMessage
from langchain.prompts import PromptTemplate, ChatPromptTemplate, HumanMessagePromptTemplate, SystemMessagePromptTemplate
from langchain.output_parsers import StructuredOutputParser, ResponseSchema
import os
from langchain.agents import load_tools
from langchain.agents import initialize_agent
from langchain.agents import AgentType
from langchain.llms import OpenAI

os.environ['OPENAI_API_KEY'] = "sk-7tZmGaqA6yEkcuAPA8C4249a86F449De82D1224388A7253c"
os.environ['SERPAPI_API_KEY'] = "ecba56657bab25863cd52e445cd4f65214afe50b1507f2a10e423293877f0516"
openai_api_key = "sk-7tZmGaqA6yEkcuAPA8C4249a86F449De82D1224388A7253c"

llm = OpenAI(model_name="gpt-4", openai_api_key=openai_api_key)

#################################工具箱应用
# # 加载一些要使用的工具
# tools = load_tools(["serpapi", "llm-math"], llm=llm)

# # 初始化 Agents
# agent = initialize_agent(tools, llm, agent=AgentType.ZERO_SHOT_REACT_DESCRIPTION, verbose=True)

# # 测试一下！
# agent.run("如何搜救大海上的遇难者？")

##################################任务分解

chat = ChatOpenAI(temperature=1, openai_api_key=openai_api_key)

response_schemas = [
    ResponseSchema(name="task", description="It represents the type of the parsed task."),
    ResponseSchema(name="id", description="The unique identifier for task planning, which is used for references to dependent tasks and their generated resources."),
    ResponseSchema(name="dep", description="It defines the pre-requisite tasks required for execution. The task will be launched only when all the pre-requisite dependent tasks are finished"),
    ResponseSchema(name="args", description="It contains the list of required arguments for task execution."),
]
output_parser = StructuredOutputParser.from_response_schemas(response_schemas)
format_instructions = output_parser.get_format_instructions()

prompt = ChatPromptTemplate(
    messages=[
        # 系统语言
            HumanMessagePromptTemplate.from_template("你是一个探索专家，要对未知可能存在潜在威胁的地图进行探索，并给出详细的任务内容和先后顺序{format_instructions}。\
                                当前可供支配的设备包括两台无人车和两架无人机 \
                                无人车：细粒度高的探索和侦查，但移动速度较慢，隐蔽性弱。无人车可进行的动作：低速抵近侦察，可以详细探索地图区域\
                                无人机：探索范围大细粒度较低，但移动速度快，隐蔽性强。无人机可进行的动作：快速巡查全图，可以完成道路构建\
                                无人机和无人车只能采取上述动作，不能使用其他动作")
        # 人类语言
        # HumanMessage(content="完成对当前地图的探索，同时保证所有设备的安全")
            ],
    input_variables=["value"],
    partial_variables={"format_instructions": format_instructions}
)
fruit_query = prompt.format_prompt(value="完成对当前地图位置位置区域的搜寻定位、路线规划和细致探索，并使无人车抵达该区域")
# print (fruit_query.messages[0].content)

fruit_output = chat(fruit_query.to_messages())
output = output_parser.parse(fruit_output.content)
print(output)
# res =  chat(
#     [
#         # 系统语言
#         SystemMessage(content="你是一个探索专家，要对未知地图进行探索，并给出详细的任务内容和先后顺序。\
#                                 当前可供支配的设备包括两台无人车和两架无人机 \
#                                 无人车：细粒度高的探索和侦查，但移动速度较慢，隐蔽性弱。无人车可进行的动作：低速抵近侦察；高细粒度地图巡视\
#                                 无人机：探索范围大细粒度较低，但移动速度快，隐蔽性强。无人机可进行的动作：快速巡查全图；发现并跟踪目标；低细粒度全图巡视\
#                                 无人机和无人车只能采取上述动作，不能使用其他动作"),
#         # 人类语言
#         HumanMessage(content="完成对当前地图的探索，同时保证所有设备的安全"),
#     ]
# )
# print(res.content)

