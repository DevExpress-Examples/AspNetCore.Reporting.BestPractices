const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:64522/';

const PROXY_CONFIG = [
  {
    context: [
      "/DXXRD", "/DXXQB", "DXXRDV"
    ],
    target,
    secure: false
  }
]

module.exports = PROXY_CONFIG;