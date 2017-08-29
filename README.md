

# IIS binding search tool CMD
##### IIS is a CMD commandline tool to search and filter IIS sites and bindings.

```
 Usage: hosts [OPTIONS] VALUE
 Shorthand for add: hosts VALUE

Copyright 2017 René Larch

  -e, --edit      Edit the hosts file in editor. set %EDITOR% to use your
                  favorite editor.
  -l, --list      List using wildcards or regex
  -a, --add       Add to hosts file
  -r, --remove    Remove from hosts file
  -f, --force     Use force (e.g. force remove)
  -i, --ip        Filter by ip address
  -n, --line      Filter by line number
  -R, --regex     Use regex for filter
  -d, --debug     Enables debuging
  --help          Display this help screen.
```
#
### Changes
```
New: Filter by domain, ip or Id.
New: Use Wildcards or Regex to filter.
New: Colorized output for regex groubs.
New: Show all sites using one IP
New: Show all sites using https://
New: Show all https sites using Sni
New: Show all https sites using CentralCertStore
New: Show all https sites without special ssl flags
New: Debug option.
```

#
### Install
If you have the chocolatey packagemanager installed, you can just run:
```CMD
choco install iis -version 1.0.0
```

if this fails you have to install chocolatey and you need to 
set your `ExecutionPolicy` to `RemoteSigned`.

#
##### I dont have chocolatey installed
Then run this:

```PowerShell
PowerShell

# remember ExecutionPolicy
$old = Get-ExecutionPolicy
Set-ExecutionPolicy RemoteSigned

# install chocolatey
iwr https://chocolatey.org/install.ps1 -UseBasicParsing | iex

# install the package
choco install iis -version 1.0.0

# reset ExecutionPolicy
Set-ExecutionPolicy $old
```


#
##### I want to use Windows 10 PackageManager
Then run this:

```PowerShell
PowerShell

# remember ExecutionPolicy
$old = Get-ExecutionPolicy
Set-ExecutionPolicy RemoteSigned

# install chocolatey
iwr https://chocolatey.org/install.ps1 -UseBasicParsing | iex

# setup provider
Install-PackageProvider -Name Chocolatey
Set-PackageSource -Name chocolatey

# install the package
Install-Package iis -version 1.0.0

# reset ExecutionPolicy
Set-ExecutionPolicy $old
```
