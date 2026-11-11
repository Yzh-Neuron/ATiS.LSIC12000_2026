dotNET_Reactor.Console -project "Confusion.nrproj"

iscc "/DMyProject=Base_LSIC12000" "/DMyAppName=LSIC12000" ./Package.iss

del /q "../Confused"

rd /q "../Confused"

Pause