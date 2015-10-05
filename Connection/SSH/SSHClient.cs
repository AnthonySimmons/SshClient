using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Renci.SshNet;


namespace Connection.SSH
{
    public class SSHClient
    {
        SshClient _sshClient;

        public bool IsConnected => _sshClient.IsConnected;

        public Stream InputStream;

        public Stream OutputStream;

        public Stream ExtStream;

        public SSHClient(string host, string port, string username, string password)
        {
            int portNum = 0;
            if(!int.TryParse(port, out portNum))
            {
                throw new Exception($"Port: {port}, must be an integer.");
            }

            try
            {
                _sshClient = new Renci.SshNet.SshClient(host, portNum, username, password);
            }
            catch (Exception)
            {
                throw;
            }
        }

        ~SSHClient()
        {
            Disconnect();
            _sshClient.Dispose();
        }

        public string Username => _sshClient?.ConnectionInfo?.Username;

        public string Cwd
        {
            get
            {
                var cwd = string.Empty;
                if(_sshClient != null && IsConnected)
                {
                    cwd = RunCommand("pwd");
                    cwd = cwd.Replace("\n", string.Empty);
                }
                return cwd;
            }
        }

        public string ClientPrompt => $"{Cwd}@{Username}$";

        public void Disconnect()
        {
            if (_sshClient.IsConnected)
            {
                _sshClient.Disconnect();
            }
        }

        public void Connect()
        {
            _sshClient.Connect();
        }

        public string RunCommand(string commandText)
        {
            if (!_sshClient.IsConnected)
            {
                throw new InvalidOperationException("Client not connected.");
            }
            var result = string.Empty;

            if (!string.IsNullOrEmpty(commandText))
            {
                var command = _sshClient.CreateCommand(commandText);
                command.Execute();
                result = command.Result;
            }
            return result;
        }

    }
}
