import logging
import ujson

import numpy as np
import pandas as pd


class BasicHandler:

    def _parse_json(self, req_json):
        return ujson.loads(req_json)

    def get_response(self, raw_data):
        data = self._parse_json(raw_data)
        return self.proceed_data(data)

    def proceed_data(self, data):
        raise NotImplementedError('Data Handler should implement `proceed_data`')


class DataFrameHandler(BasicHandler):

    def _parse_json(self, req_json):
        raw = ujson.loads(req_json)
        return raw

    @staticmethod
    def _get_avg_per_bar(df):
        per_c = (df['Close'] - df['Open']).abs() * 100 / df['Open']
        return per_c.mean()

    def proceed_data(self, data):
        # convert data into params and candles DataFrame
        params, df_ = data['st_params'], data['candles']
        df = pd.DataFrame(data=df_)
        df['Datetime'] = pd.to_datetime(df['Datetime'])

        # Some dummy logic
        level = max(df['Close'].mean(), df['Low'].max())

        logging.debug(level)

        return {'price': level}


class ListHandler(BasicHandler):

    def proceed_data(self, data):

        data_np = np.array(data)  # not necessary, just show an idea
        try:
            return self._fit(data_np)
        except Exception as e:
            logging.error(e)

    def _fit(self, data):
        min_ = np.min(data)
        max_ = np.max(data)
        return (np.mean(data) - min_) / (max_ - min_)
