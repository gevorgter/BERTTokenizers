// See https://aka.ms/new-console-template for more information

var sentence = "Record and Return to Prepared By State of Mutual of Omaha Mortgage C O DocProbe Ocean Ave Lakewood NJ Mutual of Omaha Mortgage Inc Camino del Rio North Suite San Diego CA JIM BRANNON KOOTENAI COUNTY RECORDER CDB REQ OF TITLEONE BOISE Space Above This Line For Recording Data Page of P AM RECORDING FEE Electronically Recorded FHA Case No Loan No MIN ADJUSTABLE RATE HOME EQUITY CONVERSION SECOND MORTGAGE THIS Mortgage Security Instrument or Second Security Instrument is given on The mortgagor is JOHN A HEINZ AND CYNTHIA E HEINZ HUSBAND AND WIFE whose address is N TALON LANE COEUR D ALENE Borrower Borrower is a mortgagor who is an original borrower under the Loan Agreement and Note The term Borrower does not include the Borrower s successors and assigns Mortgagor is an original mortgaor under this Security Instrument The term Mortgagor includes Mortgagor s heirs executors administrators and assigns This Security Instrument is given to the Federal Housing Commissioner whose address is Seventh Street SW DC Lender or Commissioner Borrower has agreed to repay to Lender amounts which Lender is obligated to advance including future advances under the terms of a Home Equity Conversion Mortgage Adjustable Rate Loan Agreement dated the same date as this Security Instrument Loan Agreement The agreement to repay is evidenced by Borrower s Note dated the same date as this Security Instrument Second Note This Security Instrument secures to Lender a the repayment of the debt evidenced by the Second Note with interest at a rate subject to adjustment interest and all renewals extensions and modifications of the Note up to a maximum principal amount of Five Hundred Twenty Five Thousand Dollars and Zero Cents U S b the payment of all other sums with interest advanced under Paragraph to protect the security of this Security Instrument or otherwise due under the terms of this Security Instrument and c the performance of Borrower s covenants and agreements under this Security Instrument the Second Note and Loan Agreement The full debt including amounts described in a b and c above if not due earlier is due and payable on For this purpose Borrower and Mortgagor do hereby mortgage grant bargain sell and convey to Lender the following described property located in KOOTENAI County MD HECM Second ";

var vocabulary = "./Vocabularies/base_uncased.txt";
var _tokenizer = new BERTTokenizers.BertUncasedBaseTokenizer(vocabulary);

var tokens = _tokenizer.Encode(sentence, 512, 512);
Console.Write($"'input_ids': [");
foreach ( var t in tokens)
    Console.Write($"{t.InputId}, ");
Console.WriteLine($"]");

Console.Write($"'words': [");
foreach (var t in tokens)
    Console.Write($"{t.Word}, ");
Console.WriteLine($"]");

Console.Write($"'attention_mask': [");
foreach (var t in tokens)
    Console.Write($"{t.AttentionMask}, ");
Console.WriteLine($"]");

Console.ReadKey();