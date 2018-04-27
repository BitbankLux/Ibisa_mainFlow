  //This API is created by Cygnet Infotech Pvt. Ltd.
  const responseStatus = {
    fail: 0,
    success: 1
  }

  var logger = require('../public/javascripts/logging/log');
  var truffleConfig = require('../truffle');
  
  //web3 setup
  var web3 = require('web3');
  web3 = new web3();
  var provider = new web3.providers.HttpProvider("http://" + truffleConfig.networks.development.host + ":" + truffleConfig.networks.development.port);
  web3.setProvider(provider);
  var eth = web3.eth;
  var coinbase = web3.eth.coinbase;
  
  //Setup for IBISA contract
  var contract = require("truffle-contract");
  var IBISA_artifact = require('../build/contracts/IBISA.json');
  
  var IBISA = contract(IBISA_artifact);
  IBISA.setProvider(provider);

  module.exports = function (app) {
      
    app.post('/CreateAgreement', function (req, res, next) {
      var wallet = req.body.wallet;	
      var zone = req.body.zone;
      var crop = req.body.crop;
      var maxPayout = req.body.maxPayout;
      var targetContrib = req.body.targetContrib;
      var date = req.body.date;
      var amount = req.body.amt;
      
      console.log(req.body);
      
      try {
        if (isNullOrEmpty(wallet) || isNullOrEmpty(zone) || isNullOrEmpty(crop) || isNullOrEmpty(maxPayout) || isNullOrEmpty(targetContrib)
      || isNullOrEmpty(date) || isNullOrEmpty(amount)) {
          res.send(GetFailureResponse("Please specify valid input parameters"));
          return;
        }
        
        logger.info('agreement creation Start : ');

        Promise.resolve().then(()=>{             
                IBISA.deployed().then(function (instance) {
                  return instance.createAgreement(wallet,zone,crop,web3.toWei(maxPayout,'ether'),web3.toWei(targetContrib,'ether'),date,{
                    from:wallet,value:web3.toWei(amount,'ether'),gas:6721975
                  });
                }).then(function (txHash) {
                  var responseData = {
                    status: responseStatus.success,
                    message: "Successfully created the agreement.",
                    data: {
                      tx_hash: txHash.tx
                    }
                  }
                  logger.info('agreement creation End : ');
                  res.send(responseData);
                }).catch((error)=>{
                  var err = "VM Exception while processing transaction: revert";
                  if (error.message==err){
                    res.send(GetFailureResponse("VM Exception while processing transaction: revert"));
                  }else {
                    res.send(GetFailureResponse(error.message));
                  }
                  logger.error(error);
                });
        });
      } 
      catch (error) {
        console.log(error);
        res.send(GetFailureResponse(error.message));       
      } 
    });

    app.get('/GetUserDetails/:id/:wallet', function (req, res, next) {
      var agreement_id = req.params.id;
      var wallet = req.params.wallet;
      try {
        if (isNullOrEmpty(agreement_id) || isNullOrEmpty(wallet)) {
          res.send(GetFailureResponse("Please specify valid input parameters"));
          return;
        }
        var obj = JSON.stringify(req.params);
  
        logger.info('GetAgreementDetails Start : ', obj);
        
        Promise.resolve().then(()=>{
          return IBISA.deployed().then(function (instance) {
            return instance.getUser.call(wallet)
          }).then(function (value) {
            if(value[0]==0){
              return null;
            }
  
            var BlockChainResponse = {
              totalAgreements: value[1]
            }
  
            return BlockChainResponse;
          }).catch((error)=>{
            res.send(GetFailureResponse(error.message));
            logger.error(error);
          });
        }).then((basic_data)=>{
  
          if (basic_data!=null){
            return IBISA.deployed().then(function (instance) {
              return instance.getAgreement.call(wallet,agreement_id)
            }).then(function (value) {
              if(value[0]==0){
              return null;
            }

            var temp_maxPayout = web3.fromWei(value[3], 'ether');
            var temp_totalPremiumDue = web3.fromWei(value[4],'ether');
            var temp_currentContrib = web3.fromWei(value[5],'ether');

        
            var BlockChainResponse = {
              id : value[0],
              zone : value[1],
              crop : value[2],      
              maxPayout : temp_maxPayout,
              totalPremiumDue : temp_totalPremiumDue,   
              currentContrib : temp_currentContrib,     
              dateCreatedOn : value[6]
            }		  
    
              var additional_data = basic_data;
              additional_data.agreementDetails = BlockChainResponse;
              return additional_data;
            }).catch((error)=>{
              res.send(GetFailureResponse(error.message));
              logger.error(error);
            });  
          } else {
            return null;
          }
          
        }).then((additional_data)=>{
          console.log(additional_data);
  
          if(additional_data!=null){
            return IBISA.deployed().then(function (instance) {
              return instance.getAgreement1.call(wallet,agreement_id)
            }).then(function (value) {
          if(value[0]==0){
              return null;
            }
        
          var temp = JSON.parse(value[1]) + 2592000;   //add 30 days to date of last contribution in order to get next due date
          var temp_deposit = web3.fromWei(value[2],'ether');
          var temp_withdrawal = web3.fromWei(value[3],'ether');
          var temp_merit = web3.fromWei(value[5],'ether');
          var temp_averageMerit = web3.fromWei(value[7],'ether');

              var BlockChainResponse = {
                  id : value[0],    
                  nextDueDate : temp,     
                  deposit : temp_deposit, 
                  withdrawal : temp_withdrawal,
                  merit : temp_merit,
                  periodCount : value[6],
                  averageMerit : temp_averageMerit
              }
    
              var final_data = additional_data;
              additional_data.OtherAgreementDetails = BlockChainResponse;
              return additional_data;
            }).catch((error)=>{
              logger.error(error);
              return additional_data;
            });
          } else {
            return null;
          }
        }).then((final_data)=>{
  
          if(final_data!=null) {
            var responseData = {
              status: responseStatus.success,
              message: "Data found on agreement "+agreement_id,
              data: final_data
            }
            res.send(responseData);
          } else {
            var responseData = {
              status: responseStatus.fail,
              message: "Data not found on agreement "+agreement_id
            }
            res.send(responseData);
          }
        });
        
      } catch (error) {
        logger.error(error);
        res.send(GetFailureResponse(error.message));
      }
    });

    app.post('/Indemnify/:id/:wallet/:zone', function (req, res, next) {
      var agreement_id = req.params.id;
      var wallet = req.params.wallet;	
      var zone = req.params.zone;
      
      try {
        if (isNullOrEmpty(wallet) || isNullOrEmpty(agreement_id)) {
          res.send(GetFailureResponse("Please specify valid input parameters"));
          return;
        }
    
        Promise.resolve().then(()=>{
            return IBISA.deployed().then(function (instance) {
              return instance.getIndemnity.call(wallet,agreement_id)
        });
        }).then((value)=>{
        

          if(value[0]==null) {
          return null;
          }
          var BlockChainResponse = {
            indemnity_amount : web3.fromWei(value[1],'ether')
          }
         
        return BlockChainResponse;
        }).then((data)=>{          
        
                 console.log("Indemnify process start :");
              
                   IBISA.deployed().then(function (instance) {
                     return instance.indemnify(wallet,agreement_id,zone,{
                       from:web3.eth.accounts[0],gas:6721975
                     });
                   }).then(function (txHash) {
                     var BlockChainResponse = data;
                     BlockChainResponse.tx_hash = txHash.tx;
                     console.log(BlockChainResponse);
                     var responseData = {
                       status: responseStatus.success,
                       message: "Successfully indemnified the agreement.",
                       data: BlockChainResponse
                     }
                     logger.info('Agreement indemnification End : ');
                     res.send(responseData);
                   }).catch((error)=>{
                     var err = "VM Exception while processing transaction: revert";
                     if (error.message==err){
                       res.send(GetFailureResponse("VM Exception while processing transaction: revert"));
                     }else {
                       res.send(GetFailureResponse(error.message));
                     }
                     logger.error(error);
                   });                
           })
      } 
      catch (error) {
        console.log(error);
        res.send(GetFailureResponse(error.message));      
      } 
    });
    
    app.post('/contribute/:id/:wallet/:date/:amt', function (req, res, next) {
      var agreement_id = req.params.id;
      var wallet = req.params.wallet;	
      var amount = req.params.amt;
      var date = req.params.date;

      console.log("agreement id : "+agreement_id+" wallet :"+wallet+" amount : "+amount+"date : "+date);
      
      try {

        if (isNullOrEmpty(wallet) || isNullOrEmpty(agreement_id) || isNullOrEmpty(amount) || isNullOrEmpty(date)) {
          res.send(GetFailureResponse("Please specify valid input parameters"));
          return;
        }        
        Promise.resolve().then(()=>{         
              console.log("contribute process start :");             
                IBISA.deployed().then(function (instance) {
                  return instance.contribute(wallet,agreement_id,date,{
                    from:wallet,value:web3.toWei(amount,'ether')
                  });
                }).then(function (txHash) {
                  var responseData = {
                    status: responseStatus.success,
                    message: "Successfully contributed in the agreement",
                    data: {
                      tx_hash: txHash.tx
                    }
                  }
                  logger.info('Contribute Agreement End : ');
                  res.send(responseData);
                }).catch((error)=>{
                  var err = "VM Exception while processing transaction: revert";
                  if (error.message==err){
                    res.send(GetFailureResponse("VM Exception while processing transaction: revert"));
                  }else {
                    res.send(GetFailureResponse(error.message));
                  }
                  logger.error(error);
                });
        });
      } 
      catch (error) {
        console.log(error);
        res.send(GetFailureResponse(error.message));   
      } 
    });
    
    app.get('/GetSystemDetails/:zone', function (req, res, next) {
      var zone = req.params.zone;
      try {
  
        if (isNullOrEmpty(zone)) {
          res.send(GetFailureResponse("Please specify valid input parameters"));
          return;
        }
  
        var obj = JSON.stringify(req.body);
        logger.info('GetSystemDetails Start : ', obj);
        
        Promise.resolve().then(()=>{
          return IBISA.deployed().then(function (instance) {
            return instance.countUsersInSameZone.call(zone);
          }).then(function (value) {
            if(value==0){
              return null;
            }
             
            var BlockChainResponse = {
              totalUsers: value.toNumber()
            }
            return BlockChainResponse;
          }).catch((error)=>{
            res.send(GetFailureResponse(error.message));
            logger.error(error);
          });
        }).then((basic_data)=>{
  
          if (basic_data!=null){
            return IBISA.deployed().then(function (instance) {
              return instance.indentifyUsersInSamePlot.call(zone)
            }).then(function (value) {
              if(value==0){
              return null;
            }
              
            var additional_data = basic_data;
            additional_data.listOfAddress = value;
            return additional_data;
            }).catch((error)=>{
              res.send(GetFailureResponse(error.message));
              logger.error(error);
            });  
          } else {
            return null;
          }          
        }).then((additional_data)=>{
          if(additional_data!=null){
            return IBISA.deployed().then(function (instance) {
              return instance.getUserDetailsInSameZone.call(additional_data.listOfAddress)
            }).then(function (value) {              
                var addresses = value[0];
                var agreementIds = value[1];
                var deposits = value[2];
                var withdrawals = value[3];
                
                var BlockChainResponse = [];
        
                for(var i=0;i<addresses.length;i++){
        
                  var current = {
                    wallet_address : addresses[i],
                    agreementId : agreementIds[i],
                    deposit_wallet : web3.fromWei(deposits[i],'finney'),
                    withdrawal_wallet : web3.fromWei(withdrawals[i],'finney')
                  }
                  BlockChainResponse.push(current);
                }

              var final_data = additional_data;
              additional_data.AgreementDetails = BlockChainResponse;
              return additional_data;
            }).catch((error)=>{
              logger.error(error);
              return additional_data;
            });
          } else {
            return null;
          }
        }).then((final_data)=>{
  
          if(final_data!=null) {
            var responseData = {
              status: responseStatus.success,
              message: "Data found on agreement ",
              data: final_data
            }
            res.send(responseData);
          } else {
            var responseData = {
              status: responseStatus.fail,
              message: "Data not found on agreement ",
            }
            res.send(responseData);
          }
        });
        
      } catch (error) {
        logger.error(error);
        res.send(GetFailureResponse(error.message));
      }
    });

    app.post('/createUsers', function (req, res, next) {
      try {
  
        logger.info('create users Start : ');
        
        Promise.resolve().then(()=>{
          var BlockChainResponse = [];
          
              for(var i=1;i<10;i++){
                  var current = web3.eth.accounts[i];
                  BlockChainResponse.push(current);
              }
              console.log(BlockChainResponse);
              return BlockChainResponse;
        }).then((listOfAddress)=>{
       
              console.log("user creation process start :");
                IBISA.deployed().then(function (instance) {
                  return instance.createUsers(listOfAddress,{
                    from:web3.eth.accounts[0],gas:4712388
                  });
                }).then(function (txHash) {
                  console.log(txHash);
                  var responseData = {
                    status: responseStatus.success,
                    message: "Successfully created users",
                    data: {
                      tx_hash: txHash.tx
                    }
                  }
                  logger.info('user creation completed ');
                  res.send(responseData);
                }).catch((error)=>{
                  var err = "VM Exception while processing transaction: revert";
                  if (error.message==err){
                    res.send(GetFailureResponse("VM Exception while processing transaction: revert"));
                  }else {
                    res.send(GetFailureResponse(error.message));
                  }
                  logger.error(error);
                });              
        }).catch((error)=>{
          var err = "VM Exception while processing transaction: revert";
          if (error.message==err){
            res.send(GetFailureResponse("VM Exception while processing transaction: revert"));
          }else {
            res.send(GetFailureResponse(error.message));
          }
          logger.error(error);
        });
        
      } catch (error) {
        logger.error(error);
        res.send(GetFailureResponse(error.message));
      }
    });

    app.get('/GetAllWallets', function (req, res, next) {      
      try {
        logger.info('GetAllWallets Start : ');
        
        Promise.resolve().then(()=>{                
           var BlockChainResponse = [];
        
            for(var i=1;i<10;i++){
                var current = web3.eth.accounts[i];
                BlockChainResponse.push(current);
            }
            return BlockChainResponse;
       
        }).then((list)=>{
  
          if(list!=null) {
            var responseData = {
              status: responseStatus.success,
              message: "List of wallets found",
              data: list
            }
            res.send(responseData);
          } else {
            var responseData = {
              status: responseStatus.fail,
              message: "List of wallets not found",
            }
            res.send(responseData);
          }
        });
        
      } catch (error) {
        logger.error(error);
        res.send(GetFailureResponse(error.message));
      }
    });

    app.post('/TransferEtherToSmartContract', function (req, res, next) {
      try {
        Promise.resolve().then(()=>{        
          IBISA.deployed().then(function (instance) {
            var contract_address = instance.address;
            return instance.pay.sendTransaction({
              from:web3.eth.accounts[99],
              to:contract_address,
              value:web3.toWei(80,'ether'),
              gas:6721975
            });
          }).then(function (txHash) {
            console.log(txHash);
                 var responseData = {
                    status: responseStatus.success,
                    message: "Successfully sent ethers to smart contract",
                    data: {
                      tx_hash: txHash
                    }
                  }
                  logger.info('transfer process Ends');
                  res.send(responseData);
                }).catch((error)=>{
                  var err = "VM Exception while processing transaction: revert";
                  if (error.message==err){
                    res.send(GetFailureResponse("VM Exception while processing transaction: revert"));
                  }else {
                    res.send(GetFailureResponse(error.message));
                  }
                  logger.error(error);
                });
        });
      } 
      catch (error) {
        console.log(error);
        res.send(GetFailureResponse(error.message));       
      } 
    });

    app.get('/GetTransactionReceipt/:tx', function (req, res, next) {
      var tx_hash = req.params.tx;
      try {  
        if (isNullOrEmpty(tx_hash)) {
          res.send(GetFailureResponse("Please specify valid input parameters"));
          return;
        } 
     
        logger.info('GetTransactionReceipt Start : ');
        
        Promise.resolve().then(()=>{
           var transaction = web3.eth.getTransaction(tx_hash);
           console.log(transaction);
           return transaction;
        }).then((receipt)=>{
  
          if(receipt!=null) {
            var responseData = {
              status: responseStatus.success,
              message: "TransactionReceipt found",
              data: receipt
            }
            res.send(responseData);
          } else {
            var responseData = {
              status: responseStatus.fail,
              message: "TransactionReceipt not found",
            }
            res.send(responseData);
          }
        });
        
      } catch (error) {
        logger.error(error);
        res.send(GetFailureResponse(error.message));
      }
    });
};

  
  function GetFailureResponse(message) {
    return {
      status: responseStatus.fail,
      message: message,
    };
  }  
 
  function isNullOrEmpty(value) {
    return (value == null || value.length === 0);
  }
  