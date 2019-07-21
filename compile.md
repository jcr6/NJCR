# Compiling notes

If you wish to compile this tool yourself, you will need a few tools and libraries which are not from me:


First of all VisualStudio, of course.
Then I'll assume you have git installed.... Good....
Lastly, you'll need NLua


My own libraries are dynamically linked into the project. To make sure you got themn all you will need to clone them all from github
Now the script below may work on all platforms providing that both git and the Visual Studio compilation tools are in your path variable.

~~~powershell
mkdir Tricky
cd Tricky
git clone https://github.com/jcr6/JCR6_CSharp.git JCR6
git clone https://github.com/jcr6/Tricky1975/trickyunits_csharp.git TrickyUnits
git clone https://github.com/jpbubble/NIL-isn-t-Lua.git Bubble/NIL
git clone https://github.com/jcr6/NJCR.git NJCR
~~~

Lastly you'll need to have NLua attacked to the NJCR project. Since I've never cloned and compiled other people's projects using NLua, and I just used Visual Studio to import NLua, all I can do is show you the way to go in VS:
![image](https://user-images.githubusercontent.com/11202073/61591124-e1189300-abc2-11e9-942b-e93c20316e20.png)
![image](https://user-images.githubusercontent.com/11202073/61591144-3785d180-abc3-11e9-902d-b1011d69a81f.png)
![image](https://user-images.githubusercontent.com/11202073/61591152-54baa000-abc3-11e9-976f-01e3b604f506.png)
![image](https://user-images.githubusercontent.com/11202073/61591161-71ef6e80-abc3-11e9-841d-45864dee87e0.png)

Well, and now the project should be able to be compiled without trouble.


# Note:

I only advice to compile this all yourself when you are fully aware of how Visual Studio works and when you have experience with compilers in general. If you are a complete "nitwit" in this field, you can best just wait for 
releases to appear. :)
