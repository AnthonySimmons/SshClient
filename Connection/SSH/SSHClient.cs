using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Renci.SshNet;
using System.Text;

namespace Connection.SSH
{
    public class SSHClient
    {
        SshClient _sshClient;

        SftpClient _sftpClient;

        ConnectionInfo _connectionInfo;

        public bool IsConnected => _sshClient.IsConnected;

        private const string FileUploadCommand = "put";

        private const string FileDownloadCommand = "get";


        public SSHClient(string host, string port, string username, string password)
        {
            int portNum = 0;
            if(!int.TryParse(port, out portNum))
            {
                throw new Exception($"Port: {port}, must be an integer.");
            }
                        
            try
            {
                var auth = new PasswordAuthenticationMethod(username, password);

                _connectionInfo = new ConnectionInfo(host, username, auth);
                _sshClient = new SshClient(_connectionInfo);
                _sftpClient = new SftpClient(_connectionInfo);
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
            _sftpClient.Connect();
        }



        public string ProcessCommand(string commandText)
        {

            var result = string.Empty;
            try
            {
                _sftpClient.ChangeDirectory("/");
                if (commandText.StartsWith(FileUploadCommand, StringComparison.InvariantCultureIgnoreCase))
                {
                    //put localpath remotepath
                    var arr = commandText.Split(' ');
                    UploadFile(arr[1], arr[2]);
                }
                else if (commandText.StartsWith(FileDownloadCommand, StringComparison.CurrentCultureIgnoreCase))
                {
                    //get remotepath localpath
                    var arr = commandText.Split(' ');
                    DownloadFile(arr[2], arr[1]);
                }
                else
                {
                    result = RunCommand(commandText);
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }


        public void DownloadFile(string localFilePath, string remoteFilePath)
        {
            using (var streamWriter = new StreamWriter(localFilePath))
            {
                _sftpClient.DownloadFile(remoteFilePath, streamWriter.BaseStream);
            }
        }

        public void UploadFile(string localFilePath, string remoteFilePath)
        {
            using (var streamReader = new StreamReader(localFilePath))
            {
                _sftpClient.UploadFile(streamReader.BaseStream, remoteFilePath, canOverride: true);
            }
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
