using System;
using LtiLibrary.AspNet.Outcomes.v1;
using LtiLibrary.Core.Outcomes.v1;

namespace ConsumerCertification.Controllers
{
    /// <summary>
    /// The OutcomesoApiController is hosted by the Tool Consumer and provides
    /// the functionality of the Outcomes Service API described in IMS LTI 1.1.
    /// </summary>
    public class OutcomesController : OutcomesControllerBase
    {
        // Simple "database" of scores for demonstration purposes
        private static LisResult _lisResult;

        /// <summary>
        /// Delete the score from the consumer database.
        /// </summary>
        /// <param name="lisResultSourcedId">The SourcedId of the score to delete.</param>
        /// <returns>True if the score is deleted.</returns>
        protected override bool DeleteResult(string lisResultSourcedId)
        {
            if (LisResultSourcedIdIsValid(lisResultSourcedId))
            {
                _lisResult = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Read the score from the consumer database.
        /// </summary>
        /// <param name="lisResultSourcedId">The SourcedId of the score to read.</param>
        /// <returns>The LisResult representing the score.</returns>
        protected override LisResult ReadResult(string lisResultSourcedId)
        {
            if (LisResultSourcedIdIsValid(lisResultSourcedId))
            {
                if (_lisResult == null || !lisResultSourcedId.Equals(_lisResult.SourcedId, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new LisResult { IsValid = true };
                }
                return _lisResult;
            }
            return new LisResult { IsValid = false };
        }

        /// <summary>
        /// Store the score in the consumer database.
        /// </summary>
        /// <param name="result">The LisResult to store.</param>
        /// <returns>True if the score is saved.</returns>
        protected override bool ReplaceResult(LisResult result)
        {
            if (LisResultSourcedIdIsValid(result.SourcedId))
            {
                if (_lisResult == null)
                {
                    _lisResult = new LisResult();
                }
                _lisResult.IsValid = true;
                _lisResult.Score = result.Score;
                _lisResult.SourcedId = result.SourcedId;
                return true;
            }
            return false;
        }

        private bool LisResultSourcedIdIsValid(string lisResultSourcedId)
        {
            // LisResultSourcedId is {userId}-{linkId} (see Lti1Controller)
            var sourcedId = lisResultSourcedId.Split('-');
            if (sourcedId.Length < 2)
            {
                return false;
            }
            // Only valid userIds are 1, 2, 3
            if (!sourcedId[0].Equals("1") && !sourcedId[0].Equals("2") && !sourcedId[0].Equals("3"))
            {
                return false;
            }
            // Only valid linkIds are link1, link2
            return sourcedId[1].Equals("link1") || sourcedId[1].Equals("link2");
        }
    }
}