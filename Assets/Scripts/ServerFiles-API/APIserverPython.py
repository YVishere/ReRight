from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

app = FastAPI()

class RequestData(BaseModel):
    data: str

@app.post("/process")
async def process_data(request_data: RequestData):
    if request_data.data == "GetData":
        response = "Hello from the server"
    else:
        response = "Invalid request"
    
    return {"response": response}

@app.get("/")
async def read_root():
    return {"message": "Server is listening for incoming connections"}

@app.get("/stop")
async def stop_server():
    raise HTTPException(status_code=503, detail="Server stopped")

if __name__ == "__main__":
    import uvicorn
    print("Running server...")
    uvicorn.run(app, host="127.0.0.1", port=25001)