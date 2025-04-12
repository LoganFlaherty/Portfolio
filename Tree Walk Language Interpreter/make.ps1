winget install Microsoft.OpenJDK.21
javac tool\*.java
java tool.GenerateAst lox
javac lox\*.java
java lox.Lox hello_world.lox