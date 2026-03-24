
{
  description = "F# Development Environment";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
    devenv = {
      url = "github:cachix/devenv";
      inputs.nixpkgs.follows = "nixpkgs";
    };
    flake-parts.url = "github:hercules-ci/flake-parts";
  };

  outputs =
    { self, nixpkgs, devenv, flake-parts, ... }@inputs:
    flake-parts.lib.mkFlake { inherit inputs; } {
      systems = [ "x86_64-linux" "aarch64-linux" ];
      perSystem = { pkgs, system, ... }:
        let
          dotnet = with pkgs.dotnetCorePackages; combinePackages [ sdk_10_0 ];

          tooling = with pkgs; [
            openssl
            icu
            zlib
            krb5
            stdenv.cc.cc.lib
            fsautocomplete
            fantomas
          ];
        in
        {
          _module.args.pkgs = import nixpkgs {
            inherit system;
            config.allowUnfree = true;
          };

          devShells.default = devenv.lib.mkShell {
            inherit inputs pkgs;
            modules = [
              ({ pkgs, ... }: {
                packages = tooling;

                env = {
                  ASPNETCORE_ENVIRONMENT = "Development";
                  FS_AUTOCOMPLETE_PATH = "${pkgs.fsautocomplete}/bin/fsautocomplete";
                };

                languages.dotnet = {
                  enable = true;
                  package = dotnet;
                };

                enterShell = ''
                  echo "F# Dev Environment Ready"
                  echo "  Dotnet:        $(dotnet --version)"
                  echo "  fsautocomplete: ${pkgs.fsautocomplete}/bin/fsautocomplete"
                  echo "  fantomas:       $(fantomas --version)"
                '';
              })
            ];
          };
        };

      flake = {};
    };
}
