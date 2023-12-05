using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Modelos;

namespace MisTest
{
    [TestClass]
    public class TestCocina
    {
        [TestMethod]
        [ExpectedException(typeof(FileManagerException))]
        public void AlGuardarUnArchivo_ConNombreInvalido_TengoUnaExcepcion()
        {
            //arrange
            string data = "DatosAGuardar";
            string nombreArchivo = "/";

            //act
            FileManager.Guardar(data, nombreArchivo, true);

            //assert
        }

        [TestMethod]

        public void AlInstanciarUnCocinero_SeEspera_PedidosCero()
        {
            //arrange
            string nombreCocinero = "Ramon";
            Cocinero<Hamburguesa> hamburguesero = new Cocinero<Hamburguesa>(nombreCocinero);

            //act
            int pedidos = hamburguesero.CantPedidosFinalizados;

            //assert
            Assert.AreEqual(0, pedidos);
        }
    }
}