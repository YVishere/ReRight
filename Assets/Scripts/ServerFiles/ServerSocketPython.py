import sys
import os
import socket
import threading

# Add the PythonFiles directory to sys.path
python_files_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '../../../PythonFiles'))
sys.path.append(python_files_dir)

try:
    import llamaModelFile as lmf
except Exception as e:
    print(e.msg)
    print("Error: llamaModelFile module not found in", python_files_dir)
    raise ImportError("llamaModelFile module not found in", python_files_dir)

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(('localhost', 25001))
server_socket.listen(5)  # Allow up to 20 pending connections

ob = lmf.llamaModel()

print("Server is listening for incoming connections")

def handle_client(connection, client_address):
    try:
        print("Connection from", client_address)

        data = connection.recv(1024).decode()
        if data == "GetData":
            response = ob.invoke("You are the server, say hello and something random")

        elif data.find("Invoke:::") != -1:

                ####
                ##                      0        1            2            3
                ## Encoding scheme: Invoke::: {sendText} ::: Context::: {context}
                ####
                try:
                    split = data.split(":::")
                    sendText = split[1]
                
                    if data.find("Context:::") != -1:
                        context = split[3]
                    else:
                        context = None
                    
                    response = ob.invoke(sendText, context)
                except Exception as e:
                    response = "Error: Context not found"
                
        else:
            response = "Invalid request"
        
        connection.sendall(response.encode())
        print("Response sent, ", response)
    except Exception as e:
        connection.sendall(str(e).encode())
    finally:
        connection.close()
        print("Connection closed")

try:
    while True:
        connection, client_address = server_socket.accept()
        client_thread = threading.Thread(target=handle_client, args=(connection, client_address))
        client_thread.start()
except KeyboardInterrupt:
    server_socket.close()
    print("Server stopped")