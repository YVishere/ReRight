An attempt to learn game development from scratch and attaching an LLM to control in game decision.

To-Do: Create a failsafe for server connection in case the port is occupied  
        (Dynamically) Send LLAMA generated dialogues through the servers and attach them to characters

Current Progress:
<pre>   
    - The connection is now reliable after adding delay and retry for the sys.path to be settled
    - Established communication between the LLAMA model and the Unity game
    - Created TCP server files to enable communication between Python and C# scripts  
    - Created custom movement mechanics from scratch  
    - Created a base to implement collision physics  
    - Dynamically adding components for NPCs through a script. Makes making new NPCs easier.  
</pre>

Points for future: 
<pre> 
        - ~~The server fails to connect on the first try after restarting my laptop, but connects on the second try. Possibly due to the time needed for sys path to actually be implemented.~~     
        - Unity messes up directory if a file is added as a component, so I had to use System library to get the   
          absolute path of the current directory and modify it a teensy bit  
        - I have FastAPI server set up as well but its mainly for debugging and purposes because,  
                -- HTTP API have much more overhead compared to TCP sockets  
                -- TCP sockets have more messages throughput  
                -- HTTP API would have a higher CPU usage while TCP sockets would have a higher memory usage  
        - However, it is easier to debug, add features and test with HTTP API  
</pre>

Note to self: Conda just made everything more complicated. If I ever install conda on my device in future, it will be the reason why the server sockets wouldn't work.