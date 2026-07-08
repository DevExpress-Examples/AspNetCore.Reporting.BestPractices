const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:5001';

module.exports = {
  "/DXXRD": { target, secure: false },
  "/DXXQB": { target, secure: false },
  "/DXXRDV": { target, secure: false }
};