An attempt to learn game development from scratch and attaching an LLM to control in game decision.

To-Do: Chunkify data sent to LLM  
Fix Only one msg being sent to python server  


Refinement needed:
<pre>
        - I can send one messge over my current implementation of TCP but second never goes
                --> When I was able to send multiple, I was not conserving my old clients.
                --> I would make fresh clients and streams whenever there was a request and closed them the moment I was done
        - The protocol for LLM to NPC communication is a mess
        - Check if closing the connection after every call is a good idea or not based on the overhead
                --> One possible solution is to have the connection always open and store the client address in NPC global variable
                --> Client address will be initialised at the start of the game and will be used for every request
                --> However, I will need to handle closing all connections when the game is closed
        - Currently I am sending only first 1024 bytes of data, but I need to change that to send chunks of data
</pre>

Current Progress:
<pre>   
    - The connections are alive until the end of application life.  
    - NPC dialogs now come from the LLM
    - The connection is now reliable after adding delay and retry for the sys.path to be settled
    - Established communication between the LLAMA model and the Unity game
    - Created TCP server files to enable communication between Python and C# scripts  
    - Created custom movement mechanics from scratch  
    - Created a base to implement collision physics  
    - Dynamically adding components for NPCs through a script. Makes making new NPCs easier.  
</pre>

Points for future: 
<pre> 
        -For some reason the connections are only alive if the clients are stored in a variable which does not let them be forgotten. Currently they are stored in a set()
        - Sending more bytes than mentioned in the server does not automatically parse the data.
        - Every TCP request requires a new client address. So make connect to server calls inside the request functions.
               -- I have to identify if this is caused by the python file closing the connection on its own 
        - The server fails to connect on the first try after restarting my laptop, but connects on the second try. Possibly due to the time needed for sys path to actually be implemented.      
        - Unity messes up directory if a file is added as a component, so I had to use System library to get the   
          absolute path of the current directory and modify it a teensy bit  
        - I have FastAPI server set up as well but its mainly for debugging and purposes because,  
                -- HTTP API have much more overhead compared to TCP sockets  
                -- TCP sockets have more messages throughput  
                -- HTTP API would have a higher CPU usage while TCP sockets would have a higher memory usage  
        - However, it is easier to debug, add features and test with HTTP API  
</pre>

Note to self: Conda just made everything more complicated. If I ever install conda on my device in future, it will be the reason why the server sockets wouldn't work.
