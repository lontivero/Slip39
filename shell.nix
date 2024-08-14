with import (fetchTarball https://github.com/NixOS/nixpkgs/archive/refs/tags/24.05.tar.gz) { };
mkShell {
  name = "dotnet-env";
  packages = [
    dotnet-sdk_8
  ];

  DOTNET_ROOT = "${dotnet-sdk_8}";
  DOTNET_GLOBAL_TOOLS_PATH = "${builtins.getEnv "HOME"}/.dotnet/tools";
  shellHook = ''
    export PATH="$PATH:$DOTNET_GLOBAL_TOOLS_PATH"
  '';
}
