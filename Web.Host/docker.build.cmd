@echo off
set /p imagetag=������{image}:{tag}
@echo on

docker.exe rmi %imagetag% -f
docker.exe build -t %imagetag% .

pause