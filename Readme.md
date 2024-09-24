# Terraria Midi Player [应用图标](http://i.imgur.com/a6EWzOg.png)

[[最新版本](https://img.shields.io/github/release/trigger-death/TerrariaMidiPlayer.svg?style=flat&label=version)](https://github.com/trigger-death/TerrariaMidiPlayer/releases/latest)
[[最新版本发布日期](https://img.shields.io/github/release-date-pre/trigger-death/TerrariaMidiPlayer.svg?style=flat&label=released)](https://github.com/trigger-death/TerrariaMidiPlayer/releases/latest)
[[总下载量](https://img.shields.io/github/downloads/trigger-death/TerrariaMidiPlayer/total.svg?style=flat)](https://github.com/trigger-death/TerrariaMidiPlayer/releases)
[[创建日期](https://img.shields.io/badge/created-august%202017-A642FF.svg?style=flat)](https://github.com/trigger-death/TerrariaMidiPlayer/commit/2a6570de78f8c2fd8816b8ba9380614e1badec0f)
[[泰拉瑞亚论坛](https://img.shields.io/badge/terraria-forums-28A828.svg?style=flat)](https://forums.terraria.org/index.php?threads/61257/)
[[Discord](https://img.shields.io/discord/436949335947870238.svg?style=flat&logo=discord&label=聊天&colorB=7389DC&link=https://discord.gg/vB7jUbY)](https://discord.gg/vB7jUbY)

这是一个为泰拉瑞亚乐器（如竖琴和铃铛）设计的MIDI播放器。Terraria Midi Player通过控制鼠标在屏幕上点击正确的位置来产生正确的音符，随着MIDI文件的播放。程序附带一组全局热键，当焦点在泰拉瑞亚上时可以按下这些热键以强制停止歌曲或关闭Terraria Midi Player。当开始播放歌曲时，程序还会强制将焦点切换到泰拉瑞亚，以避免因点击未知位置而引起问题。

[窗口预览](https://i.imgur.com/Sjs0sYB.png)

### [Wiki](https://github.com/trigger-death/TerrariaMidiPlayer/wiki) | [致谢](https://github.com/trigger-death/TerrariaMidiPlayer/wiki/Credits) | [图片集](http://imgur.com/a/LtTvj)

### [[获取Terraria Midi Player](http://i.imgur.com/klNsxtL.png)](https://github.com/trigger-death/TerrariaMidiPlayer/releases/latest)

## 关于

* **创作者：** Robert Jordan
* **语言：** C#, WPF

## 运行要求
* .NET Framework 4.5.2 | [离线安装程序](https://www.microsoft.com/en-us/download/details.aspx?id=42642) | [在线安装程序](https://www.microsoft.com/en-us/download/details.aspx?id=42643)
* Windows 7 或更新版本
* PC版泰拉瑞亚 <sup>(在游戏未运行时不播放MIDI)</sup>
* 泰拉瑞亚缩放设置必须为100%

[需要100%缩放](http://i.imgur.com/hZ9tm0U.png)

## 功能
* 加载MIDI或ABC记谱法文件。
* 自动尝试将音符适应泰拉瑞亚的两个八度范围内。
* MIDI自定义：
  * 启用和禁用音轨
  * 更改音轨的八度偏移
  * 更改音符偏移
  * 更改速度
* 能够控制魔法竖琴投射物的目标方向。
* 调整坐骑高度偏移。
* 使用时间设置允许更快地演奏音符。（假设您在游戏中也有修改方法）
* 所有设置在关闭窗口时保存。
* 与其他人从他们的Terraria Midi Player托管同步歌曲进行表演 *(实验性)*
* 播放歌曲时自动保持对泰拉瑞亚的焦点，以避免意外点击。（可以禁用）
* 在程序内播放MIDI，以听到它们在泰拉瑞亚中的声音。
* 查看MIDI中所有音轨的图表，了解限制在哪里造成问题。

## 小贴士
* 单击投射物角度并拖动以轻松瞄准。
* 在拖动时使用鼠标滚轮改变范围。
* [MidiEditor](http://midieditor.sourceforge.net/) 是一个免费的MIDI编辑工具。为了使大多数MIDI可播放，您可能需要它。

## 默认快捷键
* 强制关闭: `数字键盘 +` <sup>(<code>Page Up</code>, 当没有数字键盘时)</sup>
* 播放MIDI: `数字键盘 0` <sup>(<code>Delete</code>, 当没有数字键盘时)</sup>
* 暂停MIDI: `数字键盘 1` <sup>(<code>End</code>, 当没有数字键盘时)</sup>
* 停止MIDI: `数字键盘 2` <sup>(<code>Page Down</code>, 当没有数字键盘时)</sup>
* 切换坐骑偏移: `R` <sup>(仅当焦点在泰拉瑞亚上时)</sup>

## YouTube 预览
以下是一些在开发过程中录制的有关Terraria Midi Player的视频。

[[Shake It!](http://i.imgur.com/GCdPcJm.png)](https://www.youtube.com/watch?v=NsOI2k8nKbQ) [[Through the Fire and Flames](http://i.imgur.com/sHypeWL.png)](https://www.youtube.com/watch?v=BAXK9uwE_BI) [[Tal Tal Heights](http://i.imgur.com/NNsoJCG.png)](https://www.youtube.com/watch?v=rP4O6BsBEh0)
