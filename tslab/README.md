# What is it
It's simple TSLab strategy that can take some logic from python server app
It send data to python, python make some calcs and sends back some result
For sure this strategy will not make any money. Don't use it for live trading!!!

# How to use

* Start python server `waitress-serve --port=8000 server:app` in app where server.py is
Copy link to server from waitress. Replace it here ` private PyWorker pyWorker = new PyWorker("http://DESKTOP-EC18PUB:8000/tslab"); // insert here your link from python server` 
in `Strategy.cs`
* Make new solution (dll) and paste those 2 classes
* Compile (you probably need to add refs to TsLab dlls and install NewtonSoft)
* Copy output dll and Newtonsoft dll to some folder you want
* Create strategy in TsLab with external script. 
* Add output dll and Newtonsoft dll in external script
* Run strategy
* That's it