//This unit testing is created by Cygnet Infotech Pvt. Ltd.
let IBISA           = artifacts.require("./IBISA.sol");
expect              = require('chai').expect;
Web3                = require('web3'),
web3                = new Web3(),
truffleConfig       = require('../truffle'),
provider            = new Web3.providers.HttpProvider("http://" + truffleConfig.networks.development.host + 
                                                    ":" + truffleConfig.networks.development.port);

web3.setProvider(provider);

contract('Unit Test for IBISA smart contract', function(accounts){

var listOfAccounts = [web3.eth.accounts[1],web3.eth.accounts[2],web3.eth.accounts[3],web3.eth.accounts[4],web3.eth.accounts[5],web3.eth.accounts[6],web3.eth.accounts[7],web3.eth.accounts[8],web3.eth.accounts[9]],
    testAccount = web3.eth.accounts[1],
    testAccount2 = web3.eth.accounts[2],
    listOfAddress;
 

describe("Deploy the smart contract", function(){
    
    it("Get instance of deployed contract",function(){
        return IBISA.new().then(function(instance){
            IBISAContract = instance;
        });
    });

});


describe("Check pay function",function(){

    it("Transfer ether to smart contract",function(){
        return IBISAContract.pay({from:web3.eth.accounts[99],value:web3.toWei(80,"ether")}).then(function(res){
            expect(web3.eth.getBalance(IBISAContract.address).toNumber()).to.not.equal(0);
        });
    });

});

describe("Creating 10 users",function(){

    it("Create Users Function",function(){
        return IBISAContract.createUsers(listOfAccounts,{from:web3.eth.coinbase}).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });

});

describe("Get User Details",function(){

    it("Check User Details",function(){
        return IBISAContract.getUser.call(testAccount).then(function(res){
            expect(res[0]).to.be.not.equal("0x");
            expect(res[1].toNumber()).to.be.equal(0);
            expect(res[2].toNumber()).to.be.equal(0);
            expect(res[3].toNumber()).to.be.equal(0);
            expect(res[4].toString()).to.be.equal("true");
        });
    });

});

describe("Creating Agreement for a user1",function(){
    
    it("Create Agreement Function",function(){
        return IBISAContract.createAgreement(testAccount,
                                            "zoneA",
                                            "wheat",
                                            web3.toWei(50,'ether'),
                                            web3.toWei(5,'ether'),
                                            1518084000,
                                            {from:testAccount,value:web3.toWei(2,'ether')}
                                            ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });
    
});

describe("Creating Agreement for a user2",function(){
    
    it("Create Agreement Function",function(){
        return IBISAContract.createAgreement(testAccount,
                                            "zoneA",
                                            "rice",
                                            web3.toWei(50,'ether'),
                                            web3.toWei(5,'ether'),
                                            1518084000,
                                            {from:testAccount,value:web3.toWei(2,'ether')}
                                            ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });
    
});

describe("Get Agreement Details after agreement creation",function(){

    it("Get Basic Agreement Details",function(){
        return IBISAContract.getAgreement.call(testAccount,1).then(function(res){
            expect(res[0].toNumber()).to.be.not.equal(0);
            expect(res[1].toString()).to.be.not.equal('null');
            expect(res[2].toString()).to.be.not.equal('null');
            expect(res[3].toNumber()).to.be.not.equal(0);
            expect(res[4].toNumber()).to.be.not.equal(0);
            expect(res[5].toNumber()).to.be.not.equal(0);
            expect(res[6].toString()).to.be.not.equal('null');
        });
    });

    it("Get Advanced Agreement Details",function(){
        return IBISAContract.getAgreement1.call(testAccount,1).then(function(res){
            expect(res[0].toNumber()).to.be.not.equal(0);
            expect(res[1].toString()).to.be.not.equal('null');
            expect(res[2].toNumber()).to.be.not.equal(0);
            expect(res[3].toNumber()).to.be.equal(0);
            expect(res[4].toNumber()).to.be.not.equal(0);
            expect(res[5].toNumber()).to.be.not.equal(0);
            expect(res[6].toNumber()).to.be.not.equal(0);
            expect(res[7].toNumber()).to.be.equal(0);
            expect(res[8].toNumber()).to.be.not.equal(0);
        });
    });

});

describe("Contribute in the Agreement of user 1 for 4 times",function(){
    
    it("Contribute Function call 1",function(){
        return IBISAContract.contribute(testAccount,
                                        1,
                                        1520676000,
                                        {from:testAccount,value:web3.toWei(2,"ether")}
                                        ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });

    it("Contribute Function call 2",function(){
        return IBISAContract.contribute(testAccount,
                                        1,
                                        1523268000,
                                        {from:testAccount,value:web3.toWei(2,"ether")}
                                        ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });
    
    it("Contribute Function call 3",function(){
        return IBISAContract.contribute(testAccount,
                                        1,
                                        1525860000,
                                        {from:testAccount,value:web3.toWei(2,"ether")}
                                        ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });

    it("Contribute Function call 4",function(){
        return IBISAContract.contribute(testAccount,
                                        1,
                                        1531044000,
                                        {from:testAccount,value:web3.toWei(2,"ether")}
                                        ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });
});

describe("Contribute in the Agreement of user 2 for 4 times",function(){
    
    it("Contribute Function call 1",function(){
        return IBISAContract.contribute(testAccount2,
                                        1,
                                        1520676000,
                                        {from:testAccount2,value:web3.toWei(2,"ether")}
                                        ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });

    it("Contribute Function call 2",function(){
        return IBISAContract.contribute(testAccount2,
                                        1,
                                        1523268000,
                                        {from:testAccount2,value:web3.toWei(2,"ether")}
                                        ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });
    
    it("Contribute Function call 3",function(){
        return IBISAContract.contribute(testAccount2,
                                        1,
                                        1525860000,
                                        {from:testAccount2,value:web3.toWei(2,"ether")}
                                        ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });

    it("Contribute Function call 4",function(){
        return IBISAContract.contribute(testAccount2,
                                        1,
                                        1531044000,
                                        {from:testAccount2,value:web3.toWei(2,"ether")}
                                        ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });
});

describe("Get Agreement Details of user 1 upto 5 contributions",function(){    
    
    it("Get Basic Agreement Details",function(){
        return IBISAContract.getAgreement.call(testAccount,1).then(function(res){
            expect(res[0].toNumber()).to.be.not.equal(0);
            expect(res[1].toString()).to.be.not.equal('null');
            expect(res[2].toString()).to.be.not.equal('null');
            expect(res[3].toNumber()).to.be.not.equal(0);
            expect(res[4].toNumber()).to.be.not.equal(0);
            expect(res[5].toNumber()).to.be.not.equal(0);
            expect(res[6].toString()).to.be.not.equal('null');
        });
    });

    it("Get Advanced Agreement Details",function(){
        return IBISAContract.getAgreement1.call(testAccount,1).then(function(res){
            expect(res[0].toNumber()).to.be.not.equal(0);
            expect(res[1].toString()).to.be.not.equal('null');
            expect(res[2].toNumber()).to.be.not.equal(0);
            expect(res[3].toNumber()).to.be.equal(0);
            expect(res[4].toNumber()).to.be.not.equal(0);
            expect(res[5].toNumber()).to.be.not.equal(0);
            expect(res[6].toNumber()).to.be.not.equal(0);
            expect(res[7].toNumber()).to.be.equal(0);
            expect(res[8].toNumber()).to.be.not.equal(0);
        });
    });

});

describe("Get users belonging to same zone",function(){

    it("count users in a zone",function(){
        IBISAContract.countUsersInSamePlot.call("zoneA").then(function(res){
            expect(res.toNumber()).to.be.not.equal(0);
        });
    });

    it("Get wallet addresses of users in same zone",function(){
        IBISAContract.indentifyUsersInSamePlot.call("zoneA").then(function(res){           
            expect(res).to.be.not.equal('null');
            listOfAddress = res;
            //console.log(listOfAddress);
        });
    });

    it("Get details of users in same plot",function(){
        return IBISAContract.getUserDetailsInSamePlot.call([testAccount,testAccount2]).then(function(res){
            expect(res[0]).to.be.not.equal('null');
            expect(res[1]).to.be.not.equal('null');
            expect(res[2]).to.be.not.equal('null');
            expect(res[3]).to.be.not.equal('null');
        });
    });
});

describe("Indemnify user 1",function(){
    
    it("Get Indemnity amount",function(){
        return IBISAContract.getIndemnity(testAccount,1).then(function(res){
            expect(res[0].toNumber()).to.be.not.equal(0);
            expect(res[1].toNumber()).to.be.not.equal(0);
        })
    })

    it("Indemnify Function call",function(){
        return IBISAContract.indemnify(testAccount,
                                        1,
                                        "zoneA",
                                        {from:web3.eth.coinbase}
                                        ).then(function(res){
            expect(res.tx).to.be.not.equal('null');
        }); 
    });

    
});

});