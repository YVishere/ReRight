import socket

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(('localhost', 25001))
server_socket.listen(1)

print("Server is listening for incoming connections")

try:
    while True:
        connection, client_address = server_socket.accept()
        try:
            print("Connection from", client_address)

            data = connection.recv(1024).decode()
            if data == "GetData":
                response = "Hello from the server"
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