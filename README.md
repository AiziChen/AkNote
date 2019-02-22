# AkNote
一款所见即所得的富文本编辑器，适用于编程工作者、教师、作家等等...（A WYSIWYG rich notepad for computer science, teacher, author and all kinds of peolpe(AKO).) 
### 其特色之处在于：
0. 开源，下载和使用都是免费的；
1. 其具有类Word一样的所见即得的界面；
2. 笔记支持N级嵌套（分类） —— 适用于程序员、教师作为笔记本，作家用来作为写书的工具（多层级嵌套作为章节/排版丰富)；
3. 采用SQLite数据库，内容的存取速度极快，同时也解决了单个文件会产生的各种问题；主程序部分采用C#编写，同时也具有极快的运行速度；
4. 搜索也非常快速和方便（后续将加入智能搜索）；
5. 采用TinyMce，编辑功能强大；
6. 界面简洁、清爽、大方；
7. 人性化的快捷键，解决窗口同级化问题（用过之后你会忘不掉那种爽快感）；
8. 可以将笔记导出到HTML文件，以便分享或打印等。

缺点：只能在Windows平台下使用。

## 使用方法 - Usage
1. 下载并安装Visual Studio 2017（低版本未测试）
2. git clone本仓库到指定目录
3. 使用Visual Studio打开该项目
4. 安装Nuget插件（如果没有）
5. 选择Debug的x86平台运行，此时会看到软件已经启动，但编辑器界面却是空白的，接着下一步操作
6. 打开文件管理器，将源码中的tinymce文件夹复制进刚才项目生成的x86/Debug文件夹中，再次选择Debug的x86平台运行，即可看到效果

## 截图 - ScreenShot
### 1.（主界面 - Main window)
![main_window](https://github.com/AiziChen/AkNote/blob/master/screenshot/screenshot_main2.png)
### 2.（帮助和信息 - Information and help)
![information](https://github.com/AiziChen/AkNote/blob/master/screenshot/screenshot_inf.png)
