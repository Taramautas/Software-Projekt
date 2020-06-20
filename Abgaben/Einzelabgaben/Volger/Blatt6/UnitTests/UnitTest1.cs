using NUnit.Framework;

namespace NUnitTestProject1
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
       
        /*
        [Test]
        public void testGeneriereBenutzer()
        {
            string mail;
            string id;
            string pw;
            int role;

            generiereBenutzer(mail, id, pw, role);

            Assert.IsNotNull(benutzer.Get(ID), "User does not exist");
            Assert.AreEqual(mail, benutzer.getMail(id), "Mail is wrong");
            Assert.AreEqual(id, benutzer.getID(id), "ID is wrong");
            Assert.AreEqual(pw, benutzer.getPW(id), "Password is wrong");
            Assert.AreEqual(role, benutzer.getRole(id), "Role is wrong");
        }
        */
    }
}