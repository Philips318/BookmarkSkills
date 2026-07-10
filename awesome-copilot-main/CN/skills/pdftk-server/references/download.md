#下载

PDFtk为Windows提供了一个安装程序。许多Linux发行版提供了PDFtk包，您可以使用它们的包管理器下载和安装。

##微软Windows

使用以下命令下载Windows 10和11的PDFtk Server安装程序：```bash
winget install --id PDFLabs.PDFtk.Server
```
然后运行安装程序：```bash
.\pdftk_server-2.02-win-setup.exe
```
安装完成后，打开命令提示符，输入`pdftk`并按Enter。PDFtk将通过显示简短的使用信息来响应。

# # Linux

在Debian/Ubuntu-based发行版上：```bash
sudo apt-get install pdftk
```
在RedHat/Fedora-based发行版上：```bash
sudo dnf install pdftk
```
## PDFtk服务器GPL许可证

PDFtk Server （PDFtk）不是公共领域软件。它可以在其[GNU通用公共许可证（GPL）版本2]（https://www.pdflabs.com/docs/pdftk-license/gnu_general_public_license_2.txt）下免费安装和使用。PDFtk使用第三方库。[这些库的许可证和源代码在这里描述]（https://www.pdflabs.com/docs/pdftk-license/）在第三方材料下。

## PDFtk服务器再分发许可证

如果您计划将PDFtk Server作为您自己的软件的一部分分发，您将需要PDFtk Server再分发许可证。此规则的例外情况是，如果您的软件是根据GPL或其他兼容许可证向公众许可的。

商业再分发许可证允许您在遵守许可证条款的情况下，将无限数量的PDFtk Server二进制文件作为一个独特的商业产品的一部分分发。请阅读完整的许可证：

[PDFtk服务器再分发许可证（PDF）]（https://pdflabs.onfastspring.com/pdftk-server）现在售价995美元：

[PDFtk服务器再分发许可证]（https://www.pdflabs.com/docs/pdftk-license/）

##从源代码构建PDFtk服务器

PDFtk服务器可以从其源代码编译。PDFtk Server可以在[Debian](https://packages.debian.org/search?keywords=pdftk), [Ubuntu Linux](https://packages.ubuntu.com/search?keywords=pdftk), [FreeBSD](https://www.freshports.org/print/pdftk/), Slackware Linux, SuSE， Solaris和[HP-UX]（http://hpux.connect.org.uk/hppd/hpux/Text/pdftk-1.45/）上编译和运行。

下载并解压缩源代码：```bash
curl -LO https://www.pdflabs.com/tools/pdftk-the-pdf-toolkit/pdftk-2.02-src.zip
unzip pdftk-2.02-src.zip
```
查看`license_gpl_pdftk/readme.txt`中的[pdftk许可信息]（https://www.pdflabs.com/docs/pdftk-license/）。

检查为您的平台提供的Makefile，确认`TOOLPATH`和`VERSUFF`适合您的gcc/gcj/libgcj.安装。如果运行`apropos gcc`，它返回类似于`gcc-4.5`的内容，那么将`VERSUFF`设置为`-4.5`。`TOOLPATH`可能不需要设置。

进入`pdftk`子目录并运行：```bash
cd pdftk
make -f Makefile.Debian
```
根据需要替换平台的Makefile文件名。

PDFtk是使用gcc/gcj/libgcj版本3.4.5、4.4.1、4.5.0和4.6.3构建的。由于缺少libgcj特性，PDFtk 1.4x无法在gcc 3.3.5上构建。如果您使用的是gcc 3.3或更早版本，请尝试构建[pdftk 1.12]（https://www.pdflabs.com/tools/pdftk-the-pdf-toolkit/pdftk-1.12.tar.gz）。