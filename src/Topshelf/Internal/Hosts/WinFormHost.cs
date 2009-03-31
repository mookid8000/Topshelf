// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Topshelf.Internal.Hosts
{
    using System;
    using System.Windows.Forms;
    using Configuration;
    using log4net;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The actual win form host code
    /// </summary>
    public class WinFormHost
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ConsoleHost));
        private readonly IServiceCoordinator _coordinator;
        private readonly IRunConfiguration _cfg;
        private readonly IServiceLocator _serviceLocator;

        public WinFormHost(IServiceCoordinator coordinator, IRunConfiguration cfg)
        {
            _coordinator = coordinator;
            _cfg = cfg;
            _serviceLocator = ServiceLocator.Current;
        }

        [STAThread]
        public void Run()
        {
            _log.Debug("Starting up as a winform application");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _coordinator.Start();

            Form winForm = (Form)_serviceLocator.GetInstance(_cfg.FormType);
            if(winForm is ServiceConsole)
            {
                //TODO: There has to be a better way than this
                ((ServiceConsole) winForm).RegisterCoordinator(_coordinator);
            }

            _coordinator.Stopped += winForm.Close;

            Application.Run(winForm);

            _log.Info("Stopping the service");

            _coordinator.Stop();
            _coordinator.Dispose();
        }
    }
}