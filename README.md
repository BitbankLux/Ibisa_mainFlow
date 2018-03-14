# IBISA - Inclusive Blockchain Insurance using Space Assets
This repository is a first mock-up of one key function in IBISA. It is still _Work In progress_.
## Executive Summary
IBISA’s Value Proposition is: ‘_Mutual microinsurance (MMI) with low fees and fast claims payment_’. The purpose is to bring insurance (it is actually more _risk sharing_ than _insurance_) to a large and so far un-insured population worldwide (500 millions households). 

It is a huge persistent need.
### What is the Problem?
Poor, rural farmers are exposed to extreme financial distress – especially when unpredicted events occur to them.  A farmer property may be limited to a few crops, but the loss of them constitutes easily a great blow to the family’s economy. Even a small insurance contract could ensure protection and dignity. 

The problem is that small sum insured means small premium and low or negative profit for the insurers.  Therefore the business is not viable in the traditional way and the demand is not well satisfied. 
### What is the Solution?
The service IBISA (_**I**nclusive **B**lockchain **I**nsurance using **S**pace **A**ssets_) is a peer-to-peer, decentralized mutual risk sharing service, that utilizes space assets (Earth Observation) and "wisdom of the crowd", made possible by the blockchain smart contracts, to lower costs of resolving claims, extend geographic penetration and build accurate risks profiles. All this is achieved while keeping the financials profitable.

To anchor this innovative solution into the field reality and to be able to promote the associated risk sharing service, we have already engaged with several heavyweight micro-finance NGO's that will provide inputs and feedback and eventually do the promotion to their target beneficiaries.

In February 2018, IBISA received **_an award from the European Space Agency_** to further progress.

### What is the Product?
At the first stage IBISA supports risk sharing of index-based risks.

Index-based risks are the risks that a regional index (crop, drought, pests, crickets etc.) may exceed a value. IBISA uses innovative solutions to address persistent limitations that were inherited from traditional insurance practices:
* IBISA uses _blockchain-based micro-payment_ direct from each individual wallet in a form of mutualised risk sharing, to lower the cost and the risk of premium collection and to share the risk on a worldwide scale;
* IBISA uses _Earth Observation (EO) data_ to support risk and claim assessment of index-based risks;
* IBISA uses _blockchain-based community damage assessment_ (the so-called "wisdom of the crowd"), to automate and accelerate assessment, payment and lower operations costs;
* IBISA uses _blockchain smart contracts_, to bring transparency to the complete process, including the economic reward model.
### Why now?
IBISA takes advantage of the following opportunities:
1. The climate change is posing a serious threat on the small farmers and the microinsurance community is trying for at least the last 10 years for an affordable and viable economical solution;
1. The blockchain technology provides now practical solutions for micro-payments at a low cost;
1. The Earth Observation community is making available large amounts of data to support large-scale evaluation of natural disasters for free or at a reasonable cost.
## Contents of this repository
This repository is a dapp (_Work In progress_) that models one of the major functions of IBISA, the principle of peer-to-peer mutual risk sharing.

This process existed since centuries, even before insurance was invented. 
Each member of a community puts aside regularly money in a private wallet and when a disaster happens to someone, 
every member of the community pulls out a micro-amount from his wallet and contributes to indemnify the person.

The more effort someone has made to set aside money to potentially help the community, the more he can be indemnified if he has misfortune.
## Story of this mock-up
Definitions used in this model:
* a **_risk_** is a potential disaster with a certain probability (flood, drought, pest etc.);
* a **_contract_** is a coverage of a risk over a certain region for a certain type of crop;
* a **_agreement_** is an instance of a contract for a certain farmer with a certain cap of indemnity.

The story is the following:
* periodically, each farmer adds a small amount into the compartments of his wallet, similar to payment of an insurance premium;
* when an agreement  is declared "_entitled to be indemnified_" (see the story of "_watchers_" in a next mock-up) the community will be informed;
* all members of the risk sharing community will calculate the indemnity, based on a figure of merit of the victim, see [Corinthians 9:6-15](https://www.biblegateway.com/passage/?search=2%20Corinthians%209:6-15), and determine their individual contribution to the indemnity.

For this mock-up, each farmer has a wallet, with one compartment per agreement subscribed (rice, cocoa, maize, cassava etc.).
The mockup's main window displays a number of agreement compartments of farmers, 5 to 8. Each agreement is represented by a card, similarly to [the petShop tutorial of Joshua Quintal](http://truffleframework.com/tutorials/pet-shop).
Each card has fields to show (for example) the account number, the crop of this compartment, the current total of the compartment, GPS coordinates, etc.

For this mock-up, to make community indemnification more realistic there is a last wallet that represents the rest of the community and its field of current total is 10,000 times more that each individual wallet.

We can click on a button to trigger a simulated periodic premium collection. 
The latter will increase by a same amount for each agreement, in the demo, for example 100 rupees (in digital assets).
The community wallet will increase by 10,000 times the individual amount.

We can double click on a agreement of a user to indicate that this agreement is entitled for a damage indemnity.
The indemnity is calculated. 
The higher is the accumulated value of the agreement of the person who was victim of a damage, the closer is the indemnity to the capped indemnity value.
An event will be generated and the indemnity will be divided equally between all policies of the community.
Each one will transfer to its share to the indemnity to the damaged wallet.
The representative community wallet will also contribute. We should see the total of the damaged wallet increase of the value of the indemnity.
# Want to know more?

Read more about IBISA [**_here_**](http://www.bitbank.lu/ibisa)

