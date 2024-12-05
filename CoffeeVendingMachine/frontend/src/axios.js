import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5170/api',
  timeout: 10000,
});

export default api;