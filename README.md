# sitemap-generator

A C# CLI console application that generates a site map for a website.

## Description

This is a simple site map generator that takes a URL as input and creates a site map of all the pages on the website. This is meant to be used in your GitHub Actions to generate a current site map for your website as part of your deployment workflow. You can also download it and run locally on your computer.


***

## Use In GitHub Actions

- url: the url you want to generate a site map for. This is required.

- cache-key: - the cache key to upload the site map to. The defualt value is 'sitemap'. You will use the same key to download the sitemap in your deployment workflow.

- The Sitemap will be downloaded as **sitemap.xml** and will be overwritten if it already exists. This is intended behavior so your sitemap is always up to date. Logs are also downloaded as **sitemap_generator_logs.txt**


To create and upload a sitemap as an artifact use this action in your workflow

```
name: Create a Sitemap

on:
  push:
    branches: [main]

  pull_request:
    branches: [main]

jobs:
  create-site-map:
    runs-on: ubuntu-latest
    steps:
      - name: Create a Sitemap
        uses: FullStackIndie/sitemap-generator@v1.0
        with:
          url: https://example.com
          cache-key: sitemap
```

To download the sitemap use this action in your workflow

```
name: Create a Sitemap

on:
  push:
    branches: [main]

  pull_request:
    branches: [main]

jobs:
  download-site-map:
    runs-on: ubuntu-latest
    steps:
    - name: Checkout
      uses: actions/checkout@v3.5.2

    - name: Download Sitemap
      uses: actions/download-artifact@v3
        with:
          name: sitemap
          path: ./ 
```

If using Asp.Net Core your paths may look like this

```
jobs:
  download-site-map:
    runs-on: ubuntu-latest
    steps:
      - name: Download Sitemap
        uses: actions/download-artifact@v3
          with:
            name: sitemap
            path: ./app/wwwroot/
```

***

## Install Locally [Requires .Net 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

### Linux

Clone Repo into an empty Directory such as `/opt/sitemap-generator/` or `~/Workspace/Tools/sitemap-generator/`

```
mkdir /opt/sitemap-generator
cd /opt/sitemap-generator
git clone https://github.com/FullStackIndie/sitemap-generator.git .
dotnet build ./SiteMapGenerator.csproj -c Release -o ./build
cd ./build && mv SiteMapGenerator.exe sitemap.exe
```

To add sitemap-generator to the path in Linux, you can use one of the following methods:

1. Make a symlink in /usr/bin (or /usr/local/bin) directory:

    `sudo ln -s /opt/sitemap-generator/build/sitemap.exe /usr/bin/sitemap.exe`

2. Add /opt/toolname/tool.sh to $PATH variable:

    `export $PATH=$PATH:/opt/sitemap-generator/build/sitemap.exe`

3. Combine the above but use $HOME/.local/share/bin instead of /usr/bin:

```
mkdir -p $HOME/.local/share/bin
ln -s /opt/sitemap-generator/build/sitemap.exe $HOME/.local/share/bin/sitemap.exe
export PATH=$PATH:$HOME/.local/share/bin
```

Restart a new shell and You should be able to type `sitemap` and see the help menu. If so installation is successful.

***

### Windows GitBash

```

cd /c/ && mkdir sitemap-generator
cd /c/sitemap-generator && git clone https://github.com/FullStackIndie/sitemap-generator.git .
dotnet build ./SiteMapGenerator.csproj -c Release -o ./build
cd ./build && mv SiteMapGenerator.exe sitemap.exe
```

### Windows CMD

```
cd C:\ && mkdir sitemap-generator
cd C:\sitemap-generator && git clone https://github.com/FullStackIndie/sitemap-generator.git .
dotnet build ./SiteMapGenerator.csproj -c Release -o ./build
cd ./build && rename SiteMapGenerator.exe sitemap.exe
```

To add sitemap-generator to the path in Windows, you can use the following method:

Open the Start Search, type in “env”, and choose “Edit the system environment variables”

Click the “Environment Variables…” button.

Under the “System Variables” section (the lower half), find the row with “Path” in the first column, and click edit.

The “Edit environment variable” UI will appear. Here, you can click “New” and type in the new path you want to add.

```
Variable Value: C:\sitemap-generator\build
```

Click OK on all windows.

Open CMD prompt and type `echo %PATH%`

You should be able to type `sitemap` and see the help menu. If so installation is successful.

***

## CLI Usage

### Example

```
Usage: sitemap <url> [options] -P -L
sitemap https://www.example.com -P="/directory/to/save/sitemap" -L="directory/to/save/logs"
```

- First arguement is the URL of the website you want to generate a site map for. The URL must be a valid URL and must include the protocol such as `https://` or `http://`.
- -P is the path to save the sitemap.xml file. This is optional and if not specified the sitemap.xml will be saved in the current directory.
- -L is the path to save the log file. This is optional and if not specified the log file will be saved in the current directory.

| Options | Required | Default | Example Value
| :-------------- | :-------------: | ------------: | -----------: |
| url            | true     | none       | `https://www.example.com` or `http://www.example.com`
| -P or --path     | false         | Current Directory      | **'.'** or **'/var/www/html/blog'** or **'C:\Users\Me\Documents\My Website'**
| -L or --log-path     | false          | Current Directory       | **'.'** or **'/var/log'** or **'C:\Logs'**

### Linux Example

```
cd /var/www/html
sitemap https://www.example.com -P="/var/www/html" -L="/var/log"
```

### Windows GitBash Example

```
cd ~/Documents/My Website
sitemap https://www.example.com -P="/c/Users/Me/Documents/My Website" -L="/c/Logs"
```

### Windows CMD Example

```
sitemap https://www.example.com -P="C:\Users\Me\Documents\My Website" -L="C:\Logs"
```
