import torch
from langchain_ollama import OllamaLLM
from langchain_core.prompts import ChatPromptTemplate

class llamaModel:
    def __init__(self):
        self.device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
        self.model = OllamaLLM(model = "llama3.2:1b", device = self.device)
        # self.strt = "Hello world"

    def invoke(self, sendText, context = None):
        template = """
            You are simulating a person who loves to talk to people. However, you have to keep it under 500 characters.
            You can go up to 600 characters only if the context below implies that you have already responded three times and if the prompts are getting longer.
            The conversation so far is: {context}
            Your turn to speak: {sendText}
        """

        if context is None:
            response = self.model.invoke(sendText)
            return response
        
        prompt = ChatPromptTemplate.from_template(template)
        chain = prompt | self.model

        response = chain.invoke({"sendText": sendText, "context": context})

        # response = self.strt
        return response