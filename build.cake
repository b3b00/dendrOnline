#addin "Cake.Npm"

var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
///// TASKS
/////////////////////////////////////////////////////////////////////////
Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
        .Does(() =>
        {
            CleanDirectory($"./src/Example/bin/{configuration}");
            });

            Task("Build")
                .IsDependentOn("Clean")
                    .Does(() =>
                    {
                        DotNetBuild("./TreeMe.sln", new
                        DotNetBuildSettings
                            {
                                    Configuration = configuration,
                                        });
                                        });
                                        
            Task("Front")
                .Does(() => 
                {
                    var settings = new NpmInstallSettings();
                    
                            settings.LogLevel = NpmLogLevel.Info;
                            settings.WorkingDirectory = "usage/";
                            settings.Production = true;
                    
                            NpmInstall(settings);
                });                        
                                        

            Task("Test")
                .IsDependentOn("Build")
                    .Does(() =>
                    {
                        DotNetTest("./TreeMe.sln",
                        new DotNetTestSettings
                            {
                                    Configuration
                                    =
                                    configuration,
                                            NoBuild
                                            =
                                            true,
                                                });
                                                });
            
                                                //////////////////////////////////////////////////////////////////////
                                                //
                                                // EXECUTION
                                                //
                                                //////////////////////////////////////////////////////////////////////
            
                                                RunTarget(target);
