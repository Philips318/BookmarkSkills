# SOD Criteria — Design FMEA

Philips CT Design FMEA 的 Severity、Occurrence 和 Detection evaluation criteria。

---

## Severity — Design FMEA

估计 end effect 对 user 的 severity。

| 等级 | 名称 | 评价准则 | Detailed Explanation |
|-------|------|---------------------|----------------------|
| **S** | **Safety** | Safety-related failure | Refer to Hazard List。可能对 patient 或 user 造成 harm。 |
| **8** | **Critical** | Loss or degradation of primary function | - Loss of primary function（not affecting safety）<br>- Temporary loss of primary function（not affecting safety）<br>- Degradation of primary function |
| **5** | **Major** | Convenience function impact | - Product usable but comfort/convenience function inoperable（not affecting safety）<br>- Product usable but temporary loss of convenience function |
| **3** | **Minor** | Slight dissatisfaction | - Slight user dissatisfaction or annoyance<br>- Slight effect on the working or performance of the product |
| **1** | **No effect** | Not noticeable | - The user will probably not notice the effect<br>- No discernible effect |

### Standard End Effect Phrases

在 **End Effect of Failure** column 中使用这些 standard phrases：

| 严重度 | Standard Phrases |
|----------|-----------------|
| S | `Function / Incorrect image or content` (safety-relevant incorrect output) |
| 8 | `Loss of primary function`, `Temporary loss of primary function`, `Degradation of primary function` |
| 5 | `convenience function inoperable`, `temporary loss of convenience function` |
| 3 | `Slight user dissatisfaction`, `Slight effect on performance` |
| 1 | `No discernible effect` |

---

## Occurrence — Design FMEA

基于当前 prevention controls，估计 product lifetime 内 occurrence 的 probability。

依据包括 design review、analysis、simulation、testing、standards compliance，或 yield、customer complaints、failure history 等结果。

| 等级 | 名称 | 评价准则 | Likely Failure Rates |
|-------|------|---------------------|---------------------|
| **10** | **Frequent** | New technology / new design with no historical data. | ≥ 10% (1 in 10) |
| **8** | **Likely** | Failure is likely with new design, new application, or change in duty cycle / operating conditions. No design rules, supporting documentation, or relevant standards available. | 0.5% – 10% |
| **5** | **Probable** | Occasional failures associated with similar design or in design modelling and testing. | 0.02% – 0.05% |
| **3** | **Remote** | Only isolated failures associated with almost identical design or in design modelling and testing. | 0.001% – 0.02% |
| **1** | **Improbable** | Failure is eliminated through preventive control. | ≤ 0.001% |

> *上述 failure rates 是 default，但可随 business 和 product 而变化。*

### Occurrence Rating Guidelines for CT Software

| 场景 | 建议评级 |
|----------|-----------------|
| Brand new algorithm, no prior art | 10 |
| New feature on existing framework, limited testing | 8 |
| Similar feature exists on same platform, some test history | 5 |
| Minor modification to proven design, extensive test coverage | 3 |
| Mature code with years of field data, proven stable | 1 |

---

## Detection — Design FMEA

基于当前 detection controls，估计 detection probability。

使用针对该 failure mechanism 或 comparable failure mechanism 的 testing 来 characterize cause 和/或 potential failure mode，并在 design released for production 前 detect failure mode。

| 等级 | 名称 | 评价准则 |
|-------|------|---------------------|
| **10** | **Almost impossible** | No test or test procedure not capable of detecting failure prior to delivery of design for production. |
| **8** | **Remote** | Procedure is uncertain and/or there is limited experience with the new procedure. |
| **5** | **Moderate** | Proven product design and development testing procedure with new usage profile. |
| **3** | **High** | Detection of Causes (including Noise Factors) with physical testing with high confidence. |
| **1** | **Almost certain** | Design proven to conform to Standards and Best Practices, considering Lessons Learned, which effectively prevents the failure mechanism. |

### Detection Rating Guidelines for CT Software

| Detection Method | 建议评级 |
|-----------------|-----------------|
| No automated test, manual test only, untested path | 10 |
| Manual test procedure exists but limited coverage | 8 |
| Automated test suite covering the feature, proven on similar product | 5 |
| Comprehensive automated + integration tests, code review required | 3 |
| Standard-compliant design with proven prevention (architecture prevents the failure) | 1 |
