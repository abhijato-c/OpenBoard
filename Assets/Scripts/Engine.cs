using System.Diagnostics;
using System.IO;

public class Engine
{
    private Process _process = null;
    private StreamWriter _inputStream = null;
    private StreamReader _outputStream = null;

    // Spawn an engine
    public void Spawn(string path){
        // Kill prev engine
        if (_process != null && !_process.HasExited) Kill();

        // Verify path
        if (!File.Exists(path)) 
            throw new System.Exception("The specified engine does not exist! Please check the file location, and change the engine config.");
        
        ProcessStartInfo startInfo = new ProcessStartInfo {
            FileName = path,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try{
            _process = new Process { StartInfo = startInfo };
            _process.Start();
            _inputStream = _process.StandardInput;
            _outputStream = _process.StandardOutput;
            UnityEngine.Debug.Log($"Engine spawned: {path}");
        }
        catch{
            _process = null;
            _inputStream = null;
            _outputStream = null;
            throw new System.Exception("Failed to start engine! Please verify if it runs on your machine.");
        }
    }

    // Sends a command and waits for the final response line.
    public string SendCommand(string command){
        if (_process == null || _process.HasExited) return null;

        // Determine token that terminates the stread
        string terminator = null;
        if (command == "uci") terminator = "uciok";
        else if (command == "isready") terminator = "readyok";
        else if (command.StartsWith("go")) terminator = "bestmove";

        _inputStream.WriteLine(command);
        _inputStream.Flush();

        // Return immediately if the command has no response (position, uci, ready)
        if (terminator == null) return null;

        // Scan output till terminator found
        string line;
        
        while ((line = _outputStream.ReadLine()) != null){
            if (line.StartsWith(terminator)) return line;
        }
        return line;
    }

    // Terminate the engine
    public void Kill(){
        if (_process == null) return;
        if (_process.HasExited) return;
        try{
            _inputStream.WriteLine("quit");
            _inputStream.Flush();

            // Force kill if it's still there
            if (!_process.WaitForExit(100)) _process.Kill();
        }
        catch (System.Exception e){
            UnityEngine.Debug.LogWarning($"Error while killing engine: {e.Message}");
        }
        finally{
            _process.Dispose();
            _process = null;
            _inputStream = null;
            _outputStream = null;
            UnityEngine.Debug.Log("Engine killed.");
        }
    }
}