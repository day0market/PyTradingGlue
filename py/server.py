"""
This is simple falcon app that can serve logic for trading algos
On the one side we have trading terminal or another framework than
sends data to falcon server. For example it can be Multicharts/TsLab (please
look on example apps)/MetaTrader/WealthLab or any other terminal/framework that supports
HTTP requests.
This is not fully supported framework or library. It's just simple example how
you can connect things of two worlds.

My russian telegram channel https://t.me/day0market
GitHub https://github.com/day0market/
"""
import logging
import ujson
from typing import Type

import falcon

from data_handlers import DataFrameHandler, ListHandler


class URLHandler:
    def __init__(self, data_handler_cls: Type):
        self._data_handler = data_handler_cls()

    def on_post(self, req, resp):
        resp_json = self._data_handler.get_response(req.stream.read())
        logging.debug(resp_json)
        resp.status = falcon.HTTP_200
        resp.body = ujson.dumps(resp_json)


app = falcon.API()

levels = URLHandler(DataFrameHandler)
tslab = URLHandler(ListHandler)

app.add_route('/multicharts', levels)  # pandas DataFrame example
app.add_route('/tslab', tslab)  # single entry with list of data
