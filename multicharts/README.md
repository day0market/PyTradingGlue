# What is it
It's simple Multicharts strategy that can take some logic from python server app
It send data to python, python make some calcs and sends back some result
For sure this strategy will not make any money. Don't use it for live trading!!!


# How to use

* Create new strategy in Multicharts.Net editor
* Copy and paste example code
* Add reference to Newtonsoft.Json dll 
* Compile strategy
* Run python server `waitress-serve --port=8000 server:app` in app where server.py is
* Copy link to server from waitress. Paste it in strategy code and recompile
* Run multicharts strategy
* That's it :)
