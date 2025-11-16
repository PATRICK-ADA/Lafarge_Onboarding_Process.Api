-- Dummy data script for Contacts controller endpoints
-- This script inserts dummy data as if POST requests were made to the endpoints

-- Data for upload-local endpoint (Contacts table)
-- Simulating upload of an Excel/CSV file with local contacts

INSERT INTO "Contacts" ("Name", "Email", "Phone", "CreatedAt") VALUES
('John Doe', 'john.doe@lafarge.com', '+1-555-0101', NOW()),
('Jane Smith', 'jane.smith@lafarge.com', '+1-555-0102', NOW()),
('Michael Johnson', 'michael.johnson@lafarge.com', '+1-555-0103', NOW()),
('Sarah Williams', 'sarah.williams@lafarge.com', '+1-555-0104', NOW()),
('David Brown', 'david.brown@lafarge.com', '+1-555-0105', NOW()),
('Emily Davis', 'emily.davis@lafarge.com', '+1-555-0106', NOW()),
('Robert Miller', 'robert.miller@lafarge.com', '+1-555-0107', NOW()),
('Lisa Wilson', 'lisa.wilson@lafarge.com', '+1-555-0108', NOW()),
('James Moore', 'james.moore@lafarge.com', '+1-555-0109', NOW()),
('Jennifer Taylor', 'jennifer.taylor@lafarge.com', '+1-555-0110', NOW());

-- Data for upload-all endpoint (AllContacts table)
-- Simulating upload of a document file with all contact information
-- The service parses the document and creates JSON data for each category

-- Emergency contacts
INSERT INTO "AllContacts" ("Category", "Data", "CreatedAt") VALUES
('emergency', '{"Service":"Fire Department","Info":"Call 911 for emergencies"}', NOW()),
('emergency', '{"Service":"Police","Info":"Call 911 for police assistance"}', NOW()),
('emergency', '{"Service":"Ambulance","Info":"Call 911 for medical emergencies"}', NOW()),
('emergency', '{"Service":"Hospital","Info":"City General Hospital - 123 Main St"}', NOW());

-- Lafarge contacts
INSERT INTO "AllContacts" ("Category", "Data", "CreatedAt") VALUES
('lafarge', '{"Function":"Country CEO","Name":"Pierre Dupont","Phone":"+1-555-0201"}', NOW()),
('lafarge', '{"Function":"HR Director","Name":"Marie Laurent","Phone":"+1-555-0202"}', NOW()),
('lafarge', '{"Function":"Operations Manager","Name":"Jean Michel","Phone":"+1-555-0203"}', NOW()),
('lafarge', '{"Function":"Finance Director","Name":"Sophie Bernard","Phone":"+1-555-0204"}', NOW());

-- Embassy contacts
INSERT INTO "AllContacts" ("Category", "Data", "CreatedAt") VALUES
('embassies', '{"Embassy":"French Embassy","Address":"4101 Reservoir Rd NW, Washington, DC 20007","Website":"https://franceintheus.org","Phone":"+1-202-944-6000"}', NOW()),
('embassies', '{"Embassy":"Canadian Embassy","Address":"501 Pennsylvania Ave NW, Washington, DC 20001","Website":"https://www.canadainternational.gc.ca","Phone":"+1-202-682-1740"}', NOW()),
('embassies', '{"Embassy":"German Embassy","Address":"4645 Reservoir Rd NW, Washington, DC 20007","Website":"https://www.germany.info","Phone":"+1-202-298-4000"}', NOW());

-- HR contacts
INSERT INTO "AllContacts" ("Category", "Data", "CreatedAt") VALUES
('hr', '{"Name":"Alice Johnson","Designation":"HR Manager","Email":"alice.johnson@lafarge.com"}', NOW()),
('hr', '{"Name":"Bob Anderson","Designation":"Recruitment Specialist","Email":"bob.anderson@lafarge.com"}', NOW()),
('hr', '{"Name":"Carol White","Designation":"Employee Relations","Email":"carol.white@lafarge.com"}', NOW()),
('hr', '{"Name":"Daniel Lee","Designation":"Training Coordinator","Email":"daniel.lee@lafarge.com"}', NOW());