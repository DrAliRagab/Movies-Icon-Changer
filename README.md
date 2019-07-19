Download From [here](https://www.dropbox.com/s/hntjvbrq0sq48ii/IconChanger.zip?dl=0 "here")
# Movies-Icon-Changer
 [![License: AGPL v3](https://img.shields.io/badge/License-AGPL%20v3-blue.svg)](https://raw.githubusercontent.com/DrAliRagab/Movies-Icon-Changer/master/LICENSE)
 - This tool will allow you to change icons of folders of movies to more Prestigious style.

> One Picture Worth Ten Thousand Words

- It will convert this..

![](https://raw.githubusercontent.com/DrAliRagab/Movies-Icon-Changer/master/docs/3.png)
to this..
![](https://raw.githubusercontent.com/DrAliRagab/Movies-Icon-Changer/master/docs/4.png)

## How to use
#### 1. Step ONE "Essential"
1. Download [tinyMediaManager](https://github.com/tinyMediaManager/tinyMediaManager "tinyMediaManager") or get latest releases from [HERE](http://release.tinymediamanager.org/ "HERE").
2. Run **tinyMediaManager** and Search&Scrap your movies.
3. This will download poster images to your movies folders that **Movies-Icon-Changer** will use to create Icons.
4. NOW every movie folder contains `<Movie-Name>-poster.jpg`.

#### 2. Step TWO
1. Run **Movies-Icon-Changer**.
2. Select your Movies Folder.
3. Then click start.
4. This will create your movies Icons.

## How it works
- **Movies-Icon-Changer** searches for `<Movie-Name>-poster.jpg` file in each movie folder then convert it to Icon file.
- **Movies-Icon-Changer** searches for Movie rating in `*.nfo ` Files, and if not found then rating star will not be added.
- then **Movies-Icon-Changer** will assign each folder to its icon.

## Settings
- **Keyword** value must be set correctly so **Movies-Icon-Changer** will work correctly.
- Default value is `*-poster.*`, so **Movies-Icon-Changer** will search for this pattern in your movies folders.
- If you change how **tinyMediaManager** rename movie poster, so you must change **Keyword** value.
- For Example:

  - Poster name is `<Movie-Name>-poster.jpg` then Keyword value may be set to `*-poster.*`,
  - Poster name is `poster.jpg` then Keyword value may be set to `poster.jpg`,
  - Poster name is `poster.jpg or poster.png` then Keyword value may be set to `poster.*`,
  - and so on..


## Download

You can download the **latest releases** from [HERE](https://github.com/DrAliRagab/Movies-Icon-Changer/releases) 


## COPYRIGHTS

I used [The .NET library for ImageMagick: Magick.NET](https://github.com/dlemstra/Magick.NET "Magick.NET") [![GitHub license](https://img.shields.io/badge/license-Apache%202-green.svg)](https://raw.githubusercontent.com/dlemstra/Magick.NET/master/License.txt) to Convert Images to Icons.


