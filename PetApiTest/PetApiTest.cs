using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;
using PetApi;
using PetApi.Controllers;
using Xunit;

namespace PetApiTest
{
    public class PetApiTest
    {
        [Fact]
        public async Task Should_Add_Pet_When_Add_Pet()
        {
            // given
            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = testServer.CreateClient();
            Pet pet = new Pet("Baymax", "dog", "white", 5000);
            string request = JsonConvert.SerializeObject(pet);
            StringContent requestBody = new StringContent(request, Encoding.UTF8, "application/json");
            // when
            var response = await client.PostAsync("petStore/addNewPet", requestBody);
            // then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Pet actualPet = JsonConvert.DeserializeObject<Pet>(responseString);
            Assert.Equal(pet, actualPet);
        }

        [Fact]
        public async Task Should_Return_All_Pets_When_Get_All_Pets()
        {
            // given
            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = testServer.CreateClient();
            await client.DeleteAsync("petStore/clear");
            Pet pet = new Pet("Baymax", "dog", "white", 5000);
            string request = JsonConvert.SerializeObject(pet);
            StringContent requestBody = new StringContent(request, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestBody);
            // when
            var response = await client.GetAsync("petStore/pets");
            // then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actualPets = JsonConvert.DeserializeObject<List<Pet>>(responseString);
            Assert.Equal(new List<Pet>() { pet }, actualPets);
        }

        [Fact]
        public async Task Should_Return_Pet_With_Right_Name_When_Get_By_Name()
        {
            // given
            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = testServer.CreateClient();
            await client.DeleteAsync("petStore/clear");
            Pet pet = new Pet("Baymax", "dog", "white", 5000);
            string request = JsonConvert.SerializeObject(pet);
            StringContent requestBody = new StringContent(request, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestBody);
            // when
            var response = await client.GetAsync("petStore/Baymax");
            // then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actualPet = JsonConvert.DeserializeObject<Pet>(responseString);
            Assert.Equal(pet, actualPet);
        }

        [Fact]
        public async Task Should_Delete_A_Pet_When_Bought_By_A_Customer()
        {
            // given
            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = testServer.CreateClient();
            await client.DeleteAsync("petStore/clear");
            Pet pet = new Pet("Baymax", "dog", "white", 5000);
            string request = JsonConvert.SerializeObject(pet);
            StringContent requestBody = new StringContent(request, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestBody);
            // when
            await client.DeleteAsync("petStore/Baymax");
            // then
            var responseGetAllPets = await client.GetAsync("petStore/pets");
            responseGetAllPets.EnsureSuccessStatusCode();
            var responseString = await responseGetAllPets.Content.ReadAsStringAsync();
            var actualPets = JsonConvert.DeserializeObject<List<Pet>>(responseString);
            Assert.True(actualPets.Count == 0);
        }

        [Fact]
        public async Task Should_Change_Price_Of_Pet_When_Modify_Pet_Price()
        {
            // given
            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = testServer.CreateClient();
            await client.DeleteAsync("petStore/clear");
            Pet pet = new Pet("Baymax", "dog", "white", 5000);
            string request = JsonConvert.SerializeObject(pet);
            StringContent requestBody = new StringContent(request, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestBody);
            // when
            PetPriceModifyModel petPriceModifyModel = new PetPriceModifyModel(2000);
            string patchRequest = JsonConvert.SerializeObject(petPriceModifyModel);
            StringContent patchRequestBody = new StringContent(patchRequest, Encoding.UTF8, "application/json");
            await client.PatchAsync("petStore/Baymax", patchRequestBody);
            // then
            var responseGetPet = await client.GetAsync("petStore/Baymax");
            responseGetPet.EnsureSuccessStatusCode();
            var responseString = await responseGetPet.Content.ReadAsStringAsync();
            var actualPet = JsonConvert.DeserializeObject<Pet>(responseString);
            Assert.True(actualPet.Price == 2000);
        }

        [Fact]
        public async Task Should_Return_List_Of_Pets_Of_Specified_Type_When_Find_Pets_By_Type()
        {
            // given
            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = testServer.CreateClient();
            await client.DeleteAsync("petStore/clear");
            Pet petDog = new Pet("Baymax", "dog", "white", 5000);
            string requestDog = JsonConvert.SerializeObject(petDog);
            StringContent requestDogBody = new StringContent(requestDog, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestDogBody);
            Pet petCat = new Pet("Jack", "cat", "orange", 6000);
            string requestCat = JsonConvert.SerializeObject(petCat);
            StringContent requestCatBody = new StringContent(requestCat, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestCatBody);
            // when
            var response = await client.GetAsync("petStore/?type=dog");
            // then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actualPets = JsonConvert.DeserializeObject<List<Pet>>(responseString);
            Assert.Equal(new List<Pet>() { petDog }, actualPets);
        }

        [Fact]
        public async Task Should_Return_List_Of_Pets_Of_Specified_Color_When_Find_Pets_By_Color()
        {
            // given
            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = testServer.CreateClient();
            await client.DeleteAsync("petStore/clear");
            Pet petDog = new Pet("Baymax", "dog", "white", 5000);
            string requestDog = JsonConvert.SerializeObject(petDog);
            StringContent requestDogBody = new StringContent(requestDog, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestDogBody);
            Pet petCat = new Pet("Jack", "cat", "orange", 6000);
            string requestCat = JsonConvert.SerializeObject(petCat);
            StringContent requestCatBody = new StringContent(requestCat, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestCatBody);
            // when
            var response = await client.GetAsync("petStore/?color=orange");
            // then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actualPets = JsonConvert.DeserializeObject<List<Pet>>(responseString);
            Assert.Equal(new List<Pet>() { petCat }, actualPets);
        }

        [Fact]
        public async Task Should_Return_List_Of_Pets_Of_Specified_Price_Range_When_Find_Pets_By_Prince_Range()
        {
            // given
            TestServer testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = testServer.CreateClient();
            await client.DeleteAsync("petStore/clear");
            Pet petDog = new Pet("Baymax", "dog", "white", 5000);
            string requestDog = JsonConvert.SerializeObject(petDog);
            StringContent requestDogBody = new StringContent(requestDog, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestDogBody);
            Pet petCat = new Pet("Jack", "cat", "orange", 6000);
            string requestCat = JsonConvert.SerializeObject(petCat);
            StringContent requestCatBody = new StringContent(requestCat, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestCatBody);
            // when
            var response = await client.GetAsync("petStore/?minPrice=4500&&maxPrice=5500");
            // then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actualPets = JsonConvert.DeserializeObject<List<Pet>>(responseString);
            Assert.Equal(new List<Pet>() { petDog }, actualPets);
        }
    }
}
