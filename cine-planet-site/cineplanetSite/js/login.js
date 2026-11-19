const erroLogin = document.querySelector("#login-erro");

if (new URLSearchParams(window.location.search).has("erro")) {
  erroLogin.hidden = false;
  document.querySelector("#nome").focus();
}
