# NWord counter.

This bot counts how many times your messages contain the NWord in discord.

---

### Why?

I'm in a community where they use that word a lot, i don't have anything to do with it.

### implicit racism?

This bot doesn't mention nor disrespect any community or/and person, it only counts but doesn't use the NWord.

---

### Setup

To setup the bot all you need to do is compile the source using `Net7.0`, You can get that from [here](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).

To compile the source use the `Dotnet CLI` that should have been installed with downloading `Net7.0` in the previous step, to do so, run the following command in your terminal:

Assuming your working directory is already the root of this program
```bash
arch="linux-x64"; # change this for your target system architecture.

mkdir "./build";  
dotnet publish -a $arch -c Release -f net7.0 -o "./build";
```

When that commands finish executing you will see a new folder called `build` in the root of your project, open that or move to it using `cd ./build` and you will see an executable with the name `NwordCounter` or `NwordCounter.exe` depending on the target architecture.

Before building create a new file in there called `.secrets` and add the following line to it

```env
TOKEN=YOURTOKEN
```

Replace `YOURTOKEN` for a valid application token you can get from the [discord developer portal](https://discord.com/developers), create a new application, with it a new Bot and replace the token, if you don't know how use this [tutorial](https://discordnet.dev/guides/getting_started/first-bot.html) till you are able to invite your bot to your guild.

### Running the bot

You can either only run the application attached to your terminal or use a process detacher like `PM2`, you normally install `PM2` from `NPM` so we are going to do that.

Download `NODE.JS` from [here](https://nodejs.org/en) and follow the setup, **IMPORTANTLY** adding the program to the path, check the option in the setup.

Once you have `NODE.JS` installed check you did it all correctly by running

```bash
node -v
```

in your terminal, if you see a version name it means you did everything correctly.

Now that you have `NODE.JS` installed you can install `PM2` running the following in your terminal:

```bash
npm install -g pm2
```

When the installation finishes you can just run `PM2` from your terminal.

Assuming your working directory is still the bot `build` folder you are going to run the following commands

```bash
chown ./NwordCounter;
chmod 707 ./NwordCounter;
pm2 start ./NWordCounter;
```

And you should see that the process started, you can check the console logs anytime by using

```bash
pm2 logs 0 # assuming your process ID is 0.
```

If you want to stop the bot just use

```bash
pm2 stop 0;
pm2 delete 0;
```
--- 
# Disclaimer

The bot data becomes your responsibility when you host it, i won't have anything to do if you have problems because the bot is "racist" or anything, use it at your own.