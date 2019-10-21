# RZWatchDog
## the watch dog windows service  
you can use it to watch your process,if not exist in system,it can start it,include window app or not  
after you build:  
config the file RZWatchDog.exe.config find the item `ProcessPath` like to start `c:\\qq.exe` and `d:\\wechat.exe` you can config like this:
`0,c:\\qq.exe|0,d:\\wechat.exe` the `0` is wait to start time
