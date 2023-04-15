# Iberian Technology Summit 2023 - Introduction
Introduction to Fusion Teams Development for ITS 2023

Pre-requisites:

  1. Azure subscription: [Free azure account](https://azure.microsoft.com/en-us/free/)
  2. Access to a Power Platform Environment: [Power Apps Development Plan](https://learn.microsoft.com/en-US/power-apps/maker/developer-plan)
  3. Access to Visual Studio or Visual Studio Code


In the session we propose the following Use Case:

![image](https://user-images.githubusercontent.com/18615795/181196622-dfe5f539-5cfe-4b48-9eda-0adb1384891c.png)

We want to enable Citizen Developers to create a custom connector that allows them to use a Web API that exposes some internal company services. In this case, we want to create a custom connector that accesses an API that allows to manage the tracking and management of time on projects by the company's staff.

The components to be used in the Back-End are the following:

   1. [APIM](https://azure.microsoft.com/en-us/services/api-management/): Enables you to deploy API gateways in parallel with APIs hosted in Azure, in other clouds and in the on-premises environment to optimize API traffic flow. Meet security and compliance requirements with a unified management experience and complete visibility into all internal and external APIs.
   2. [App Service](https://azure.microsoft.com/en-us/services/app-service/): Compile, deploy and scale web applications and APIs automatically on your terms. In our case, we will create, publish and host a web API with .NET Core.
   3. [Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/): Azure Cosmos DB is a fully managed serverless NoSQL database for high-performance applications of any size or scale.

The components that we are going to use in the Front-End are the following:

1. [Custom Connector](https://docs.microsoft.com/en-US/connectors/custom-connectors/): Allows the API to be used from any application (Power App) or flow (Power Automate) in the environment where it is installed.
2. [Power App](https://powerapps.microsoft.com/en-US/): Application to manage the tracking of activities and time spent on projects by employees of the company.
3. [Power Automate](): Flows used to implement the different business rules.



