# shell.nix

with import /home/lontivero/Projects/nixpkgs { config.allowUnfree = true; };
let
  # jb = import /home/lontivero/Projects/nixpkgs { config.allowUnfree = true; };
  libs = [
  ];
  dependencies = [
    dotnet-sdk_8
    jetbrains.rider
  ];

in
mkShell {
  name = "dotnet-env";
  packages = dependencies;
  buildInputs = libs;

  DOTNET_ROOT = "${dotnet-sdk_8}";
  DOTNET_GLOBAL_TOOLS_PATH = "${builtins.getEnv "HOME"}/.dotnet/tools";
  #DOTNET_ROLL_FORWARD = "latestPatch";
  shellHook = ''
    export PATH="$PATH:$DOTNET_GLOBAL_TOOLS_PATH"
  '';
}
