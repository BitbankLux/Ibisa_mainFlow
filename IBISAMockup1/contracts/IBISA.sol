//This contract is created by Cygnet Infotech Pvt. Ltd.
pragma solidity ^0.4.15;

contract IBISA {
    //This creates an array of all users in system   
    mapping(address=>User) users;  

    //This creates an array of all zones and corresponding users who share agreement in that zone
    mapping(string=>User[]) zones;   
    
    //public variables of IBISA
    uint public decay;
    uint public period;  
    uint public usercount;
    uint public blackoutPeriod; 

    //This creates a structure to store User Details
    struct User {
        address wallet;   
        uint index;       
        uint deposit;     
        uint withdrawal;  
        bool isAdded;     
        mapping(uint=>Agreement) agreements;  
    }
    
    //This creates a structure to store Agreement Details of User
    struct Agreement {
        uint id;
        string zone;
        string risk;             
        uint maxPayout;          
        uint targetContrib;      
        uint indemnity;          
        uint dateLastContrib;    
        uint currentContrib;     
        uint deposit;            
        uint withdrawal;         
        uint resMerit;           
        uint merTotal;           
        uint periodCount;
        uint merit;              
        uint mFactor;
		uint dateCreatedOn;
    }
    
    /**
     * Constructor function
     *
     * Initializes contract with initial values of variables used in merit calculation
     */
    function IBISA() {
        usercount = 0;
        decay = 2;           //0.2% decay
        period = 30;         //1 month = 30 days
        blackoutPeriod = 4;  //4 months = 120 days
    }

    /**
     * Create Users
     *
     * Add users in IBISA with '_list' as list of ethereum wallet addresses
     *
     * @param _list List of addresses of users
     */
    function createUsers(address[] _list) returns (bool) {       
        uint length = _list.length;
        for (uint i = 0;i < length;i++) {
            require(_list[i] != 0x0);
            User memory user1 = users[_list[i]];  
            require(user1.isAdded == false);
            usercount += 1;        
            User memory user2 = User(_list[i],0,0,0,true);
            users[_list[i]] = user2;
        }
        return(true);
    }     

    /**
     * Create Users
     *
     * Create Agreement for a user in IBISA with initial parameters and pay first his/her contribution
     *
     * @param _wallet Address of user
     * @param _zone Geographic zone 
     * @param _risk Hash of risk descriptor
     * @param _maxPayout Maximum payout 
     * @param _targetContrib Idle Amount of premium the user should pay
     * @param _date Date of Agreement creation and the date for first Contribution 
     */
    function createAgreement(
        address _wallet,
        string _zone,
        string _risk,
        uint _maxPayout,   
        uint _targetContrib, 
        uint _date
        ) payable returns (bool) 
    {
            User storage user = users[_wallet];
            user.index += 1;
            uint temp = (msg.value*20)/100;            
            Agreement memory agreement = Agreement(user.index,
            _zone,
            _risk,
            _maxPayout,
            _targetContrib,
            0,
            _date,
            msg.value,     
            msg.value,     
            0,
            temp,          
            temp,
            1,
            0,
            20,
			_date);            
            //****************Here we consider that there is only one agreement per user
            //****************Hence only when deposit of agreement is updated,deposit of user gets updated too
            //****************It should be actually added
            user.deposit = agreement.deposit;
            user.agreements[user.index] = agreement;            //Add agreement to list of users
            zones[_zone].push(user);                          //Add user to a plot
			return (true);
    }

    /**
     * Contribute in the agreement
     *
     * User contributes in his/her agreement,Deposit wallet is increased and Merit Parameters are updated
     *
     * @param _wallet Address of user
     * @param _id Agreement Id 
     * @param _date Date for Contribution 
     */
    function contribute(address _wallet, uint _id, uint _date) payable returns(bool) {
        User storage user = users[_wallet];
        Agreement storage agreement = user.agreements[_id];       
        agreement.currentContrib = msg.value;
        agreement.deposit += msg.value;      
         
        //payment on time, payment is on or before the due date, also a delay of 10 days is allowed
        if ((_date - agreement.dateLastContrib) <= period * 1 days + 10 days ) {
            agreement.periodCount += 1;
            
            if(agreement.mFactor < 100) {
                agreement.mFactor += 20;
            }
            else {
               agreement.mFactor = 100; //no need to set 100, it will be 100 only
            }
            agreement.resMerit = (msg.value * agreement.mFactor) / 100;
           
            agreement.merTotal += agreement.resMerit;
        }
        else {
            if((_date - agreement.dateLastContrib) > period * 1 days + 10 days) {
                uint temp;
                uint difference = _date-agreement.dateLastContrib;
                uint diffInDays = difference/86400; //1 day = 86400 seconds
                uint diffInMonths = diffInDays/period;   //period = number of days
                agreement.periodCount = agreement.periodCount + diffInMonths;
                
                //decrease merTotal of previous months
                    for(uint i=0;i<diffInMonths;i++) {
                        temp = (agreement.merTotal*decay)/100;
                        agreement.merTotal = agreement.merTotal - temp;
                    }
                
                //difference greater than blackout period : decrease merit of previous months by decay,reset mFactor as 20 and calculate merit of current month
                if(diffInMonths >= blackoutPeriod) {
                    agreement.mFactor = 20;
                    agreement.resMerit = (msg.value * agreement.mFactor) / 100;
                    agreement.merTotal = agreement.merTotal + agreement.resMerit;
                }
                else { 
                    //difference lesser than blackout period : decrease merit of previous months by decay,mFactor remains as it and calculate merit of current month
                    //agreement.mFactor = 100; //its 100 only no need to set                    
                    if(agreement.mFactor < 100) {
                        agreement.mFactor += 20;
                    }
                    else {
                        agreement.mFactor = 100; //no need to set 100, it will be 100 only
                    }
                    agreement.resMerit = (msg.value * agreement.mFactor) / 100;
                    agreement.merTotal = agreement.merTotal + agreement.resMerit;
                }
            }
        }
        agreement.dateLastContrib = _date;
        user.deposit = agreement.deposit;  
        //****************Here we consider that there is only one agreement per user
        //****************Hence only when deposit of agreement is updated,deposit of user gets updated too
        //****************It should be actually added   
		return(true);
    }    
        
    /**
     * Indemnify the User
     *
     * User gets indemnified in his/her agreement, Withdrawal wallet is increased and deposit wallets of other users are decreased
     *
     * @param _wallet Address of user
     * @param _id Agreement Id 
     * @param _zone Geographic zone  
     */
    function indemnify(address _wallet, uint _id, string _zone) returns (bool) {
        User storage user = users[_wallet];
        Agreement storage agreement = user.agreements[_id];
        
        agreement.merit = agreement.merTotal / agreement.periodCount;        
        agreement.indemnity = (agreement.merit * agreement.maxPayout) / agreement.targetContrib;
        agreement.withdrawal += agreement.indemnity;

        //transfer funds from smart contract to user
        _wallet.transfer(agreement.indemnity);
        user.withdrawal += agreement.indemnity;

        bool status = reduceDeposit(agreement.indemnity,_wallet,_zone);

        return (status);
    }

    /**
     * Internal function to reduce amounts from Deposit wallets at time of Indemnity, only can be called by this contract
     */
    function reduceDeposit(uint _indemnity, address _wallet, string _zone) internal returns (bool) {
        uint len = countUsersInSameZone(_zone);
        uint indemnityPerPerson = _indemnity/(len-1);
        address[] memory list2 = indentifyUsersInSameZone(_zone);
        
        for (uint i = 0; i < len; i++) {            
            if (list2[i] != _wallet) {
                User storage user1 = users[list2[i]];
                Agreement storage agreement1 = user1.agreements[1];
                
                if (agreement1.deposit >= indemnityPerPerson) {
                    agreement1.deposit -= indemnityPerPerson;
                    user1.deposit -= indemnityPerPerson;
                }
            }
        }
		return(true);
    }   
  
    /**
     * Identify users sharing risks in same zone
     *
     * Returns list of wallet addresses of users
     *    
     * @param _zone Geographic zone  
     */
    function indentifyUsersInSameZone(string _zone) returns (address[]) {        
        User[] storage list = zones[_zone];        
        uint length = list.length;        
        address[] memory list2 = new address[](length); 
        for (uint i = 0; i < length; i++) {
            list2[i] = list[i].wallet;
        }
        return list2;        
    }

    /**
     * Get user details sharing risks in same zone
     *
     * Returns details of each user like wallet address, agreement Id, deposit wallets, withdrawal wallets
     *    
     * @param _list List of addresses of users sharing risks in same zone
     */
    function getUserDetailsInSameZone(address[] _list) returns (address[], uint[], uint[], uint[]) {       
        uint length = _list.length;
       
        address[] memory wallets  = new address[](length);
        uint[] memory agreementIds = new uint[](length);
        uint[] memory deposits = new uint[](length);
        uint[] memory withdrawals = new uint[](length);
        
        for (uint i = 0; i < length; i++) {
             User memory user = users[_list[i]];
             //Agreement memory agreement = user.agreements[1];
             wallets[i] = user.wallet;
             agreementIds[i] = 1;
             deposits[i] = user.deposit;
             withdrawals[i] = user.withdrawal;        
        }
        return(wallets,agreementIds,deposits,withdrawals);
    }     

    /**
     * Count number of users sharing risks in same zone
     *   
     * @param _zone Geographic zone  
     */
	function countUsersInSameZone(string _zone) constant returns (uint) {        
        User[] storage list = zones[_zone];
        return list.length;
    }   

    /**
     * Pay function
     *
     * To initially fund smart contract with some ethers so that agreements are successfully indemnified
     *   
     */
    function pay() public payable {
        
    }   
    
    /**
     * Get basic agreement details
     *
     * Returns details of agreement like id, zone, risk, maxPayout etc.
     *    
     * @param _wallet Address of user
     * @param _id Agreement Id 
     */
    function getAgreement(address _wallet, uint _id) constant returns(
        uint,
        string, 
        string,
        uint,
        uint,  
        uint,
        uint) 
    {
        User storage user = users[_wallet];
        Agreement memory agreement = user.agreements[_id];         
        return(
        agreement.id,
		agreement.zone,
		agreement.risk,
        agreement.maxPayout,
        agreement.targetContrib,
        agreement.currentContrib,
		agreement.dateCreatedOn
        ); 
    }        

    /**
     * Get more agreement details
     *
     * Returns details of agreement like id, deposit, withdrawal wallets,merit etc.
     *    
     * @param _wallet Address of user
     * @param _id Agreement Id 
     */    
    function getAgreement1(address _wallet, uint _id) constant returns(
        uint,
        uint,   
        uint,
        uint,
        uint,
        uint,
        uint,
        uint,
        uint) 
    {
        User storage user = users[_wallet];
        Agreement memory agreement = user.agreements[_id];         
        return(
        agreement.id,    
        agreement.dateLastContrib,
        agreement.deposit,
        agreement.withdrawal,
        agreement.resMerit,
        agreement.merTotal,
        agreement.periodCount,
        agreement.merit,
        agreement.mFactor);
    }    
    
    /**
     * Get User details
     *
     * Returns details of user like number of agreements, deposit, withdrawal wallets etc.
     *    
     * @param _wallet Address of user
     */    
    function getUser(address _wallet) constant returns(address, uint, uint, uint,bool) {
        User memory user = users[_wallet];
        return(user.wallet,user.index,user.deposit,user.withdrawal,user.isAdded);
    }    
    
    /**
     * Get indemnity amount for an agreement of user
     *    
     * @param _wallet Address of user
     * @param _id Agreement Id 
     */    
    function getIndemnity(address _wallet, uint _id) constant returns (uint, uint) {
        User storage user = users[_wallet];
        Agreement memory agreement = user.agreements[_id];        
        agreement.merit = agreement.merTotal / agreement.periodCount;        
        agreement.indemnity = (agreement.merit * agreement.maxPayout) / agreement.targetContrib;        
        return(agreement.id,agreement.indemnity);
    }    
}

