SELECT * FROM Event;
SELECT * FROM Category;
SELECT u.FullName, e.EnrolmentId, c.CategoryName, e.BibNumber, e.Status
FROM Enrolment e
JOIN ParticipantProfile pp ON pp.ParticipantProfileId = e.ParticipantProfileId
JOIN [User] u ON u.UserId = pp.UserId
JOIN Category c ON c.CategoryId = e.CategoryId;