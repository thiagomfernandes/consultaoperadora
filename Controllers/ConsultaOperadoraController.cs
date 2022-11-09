using System;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ConsultaOperadora.Models;

namespace ConsultaOperadora.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ConsultaOperadoraController : Controller
    {        
        public JsonResult Consulta(string telefone)
        {
            try
            {
                if (string.IsNullOrEmpty(telefone))
                    throw new Exception("TELEFONE NÃO INFORMADO");

                var cookie = new CookieContainer();
                var data = string.Format("tipo=consulta&numero={0}", telefone);
                var consultaSource = WebAccess.GetStringFromUrlExternal("http://consultaoperadora.com.br/site2015/resposta.php", ref cookie, "POST", data, referer: "http://consultaoperadora.com.br/site2015/");

                while(!string.IsNullOrEmpty(consultaSource))
                {
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.OptionFixNestedTags = true;
                    htmlDoc.LoadHtml(consultaSource);

                    var spansResult = htmlDoc.DocumentNode.SelectNodes("//span[@class='lead laranja']");
                    if (spansResult != null && spansResult.Count > 2)
                    {
                        if (spansResult[0].InnerText.Contains("Aguarde"))
                        {
                            Thread.Sleep(int.Parse(spansResult[1].InnerText.Trim()) * 1000);
                            consultaSource = WebAccess.GetStringFromUrlExternal("http://consultaoperadora.com.br/site2015/resposta.php", ref cookie, "POST", data, referer: "http://consultaoperadora.com.br/site2015/");
                        }
                        else
                        {
                            return Json(new
                            {
                                data_consulta = DateTime.Now.ToString(),
                                telefone = telefone,
                                operadora = HttpUtility.HtmlDecode(spansResult[0].InnerText.Trim()),
                                portabilidade = HttpUtility.HtmlDecode(spansResult[1].InnerText.Trim())
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                return Json(new
                {
                    data_consulta = DateTime.Now.ToString(),
                    telefone = telefone,
                    erro = "NÃO ENCONTRADO"
                }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    data_consulta = DateTime.Now.ToString(),
                    telefone = telefone,
                    erro = ex.Message
                }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}