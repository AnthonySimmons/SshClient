using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connection.SSH;

namespace Shell
{
    class ConnectionRunner
    {
        SSHClient _sshClient;

        public void Connect()
        {
            var host = ConsoleRunner.GetHost();
            var port = ConsoleRunner.GetPort();
            var username = ConsoleRunner.GetUsername();
            var password = ConsoleRunner.GetPassword();

            _sshClient = new SSHClient(host, port, username, password);
            _sshClient.Connect();
        }

        public void Run()
        {
            Connect();
            
            while(true)
            {
                var input = ConsoleRunner.Prompt(_sshClient.ClientPrompt);

                if(string.Compare(input, "exit", ignoreCase: true) == 0)
                {
                    break;
                }

                var result = _sshClient.ProcessCommand(input);
                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine(result);
                }
            }
        }
    }
}
