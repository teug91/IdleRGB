# IdleRGB <img align="right" src="IdleRGB/Resources/rgb.ico" width="128" style="margin:0px 30px">
Changes LEDs on Corsair keyboard, mouse, headset, and mousemat. Turns off all LEDs (or to a specified color) after a specified time of no activity from keyboard and mouse.  All the LEDs will also change to a another specified color when Caps Lock is activated. No changes are made to media, win lock, or brightness LEDs for the devices that has them. Idle time can be changed by right clicking tray icon.

This application has ONLY been tested with the [Corsair K70 RGB](http://www.corsair.com/en-us/corsair-gaming-k70-rgb-mechanical-gaming-keyboard-cherry-mx-red) and the [Corsair Scimitar](http://www.corsair.com/en-us/scimitar-rgb-optical-moba-mmo-gaming-mouse). It should however work with any of the Corsair devices supported by the [CUE SDK](http://forum.corsair.com/v3/showthread.php?t=156813). [Corsair CUE](http://www.corsair.com/en-us/landing/cue) needs to be running with the SDK option enabled.

## Credit
* [CUE.NET](https://github.com/DarthAffe/CUE.NET) - C# (.NET) Wrapper library around the Corsair CUE SDK
* [Hardcodet.NotifyIcon.Wpf](http://www.hardcodet.net/wpf-notifyicon) - Used for tray icon
* [Extended WPF Toolkit™](https://github.com/xceedsoftware/wpftoolkit) - Used for color canvas
* [Joel Abrahamsson's post](https://http://joelabrahamsson.com/detecting-mouse-and-keyboard-input-with-net/t) - Code for detecting keyboard and mouse input
