import sys
import os

# Add the PythonFiles directory to sys.path
python_files_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '../../../PythonFiles'))
sys.path.append(python_files_dir)

try:
    import llamaModelFile as lmf
except Exception as e:
    print(e.msg)
    print("Error: llamaModelFile module not found in", python_files_dir)
    raise ImportError("llamaModelFile module not found in", python_files_dir)

import socket

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(('localhost', 25001))
server_socket.listen(1)

ob = lmf.llamaModel()

print("Server is listening for incoming connections")

try:
    while True:
        connection, client_address = server_socket.accept()
        try:
            print("Connection from", client_address)

            data = connection.recv(1024).decode()
            if data == "GetData":
                # response = "Hallo"
                response = ob.invoke("You are the server, say hello and something random")

            # elif data.find("Invoke:") != -1:

            #     ####
            #     ##                      0        1            2            3
            #     ## Encoding scheme: Invoke::: {sendText} ::: Context::: {context}
            #     ####

            #     split = data.split(":::")
            #     sendText = split[1]

            #     if data.find("Context:") != -1:
            #         context = split[3]
            #     else:
            #         context = None
                
            #     response = ob.invoke(sendText, context)

            else:
                response = "Invalid request"
            
            connection.sendall(response.encode())
            print("Response sent, ", response)
        finally:
            connection.close()
            print("Connection closed")
except KeyboardInterrupt:
    server_socket.close()
    print("Server stopped")