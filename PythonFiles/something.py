from langchain_ollama import OllamaLLM
import torch

import time

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

model = OllamaLLM(model = "llama3.2:1b", device = device)

startTime = time.time()

print("Starting up...", startTime)

sendText = "Try to simulate a conversation between two people with different personalities. Each person should speak somewhere around 6-10 sentences."

response = model.invoke(sendText)
print(response)

print("Ended at: ", time.time() - startTime)
