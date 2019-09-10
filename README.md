# PyTradingGlue

This is simple solution example how to make communication between
some trading terminal or framework and python real

## How it works
On python side we will create http server with falcon (because it's fastest wf and pretty simple)
On base trading app side (Multicharts, TsLab, MetaTrader) we will use
POST HTTP requests to send data and get some calculated values

`data_handlers.py` is a file where all data processing logic (or maybe trading logic) lives
`server.py` is an entry point for server. All url handlers should be here. Please refer to falcon docs

In this example we have two endpoints. One for Multicharts strategy example and another one for TsLab strategy.
One shows how to work with simple input data as list of prices and another one shows how to work with more complicated data:
dict of params and prices

## Installation

* clone this repo
* make virtual env
* install requirements `pip install -r requirements.txt`

## Start python server
* `cd py` go down to py folder from root project folder
* `waitress-serve --port=8000 server:app`
* go to multicharts/tslab folders to find another instructions


# Disclaimer
This is made only for demonstration purpose. If you run any strategy from this repo with real trading
account you WILL LOSE MONEY. So just play with simulated market data or just use it to copy some code. Some parts of code 
might be not optimal and I will not support this repo in future. I can give you any promise that everything will work 
smoothly on your machine. Sorry for that and try to fix all issues by yourself.
 