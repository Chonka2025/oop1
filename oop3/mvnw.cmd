@REM Maven Wrapper startup script for Windows
@echo off
set MAVEN_OPTS=-Xmx512m

set WRAPPER_JAR=D:\oop1\oop3\.mvn\wrapper\maven-wrapper.jar
set WRAPPER_LAUNCHER=org.apache.maven.wrapper.MavenWrapperMain

if exist %WRAPPER_JAR% goto execute

echo.
echo Error: Maven Wrapper not found.
goto end

:execute
java -jar %WRAPPER_JAR% %*

:end
@REM pause is optional