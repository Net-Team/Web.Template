@echo off
set /p imagetag=«Î ‰»Î{image}:{tag}
@echo on

docker.exe rmi %imagetag% -f
docker.exe build -t %imagetag% .

pause